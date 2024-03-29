﻿using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Smo.Wmi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ServiceProcess;
using Microsoft.Win32;
using System.Windows.Data;
using Microsoft.IdentityModel.Tokens;
using System.Windows.Input;
using System.Data.Common;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
//using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Data.Entity;

namespace HydroApp
{

    internal class UserSettings : ViewModel
    {
        private string? servername;
        private string? maindbname;
        private string? userdbname;

        private bool autologin;
        private string? username;
        private string? password;

        //technical
        private int connectionTimeout = 15;

        public string? ServerName
        {
            get => servername;
            set
            {
                servername = value;
                OnPropertyChanged(nameof(ServerName));
            }
        }
        public string? MainDbName
        {
            get => maindbname;
            set
            {
                maindbname = value;
                OnPropertyChanged(nameof(MainDbName));
            }
        }
        public string? UserDbName
        {
            get => userdbname;
            set
            {
                userdbname = value;
                OnPropertyChanged(nameof(UserDbName));
            }
        }

        public bool Autologin
        {
            get => autologin;
            set
            {
                autologin = value;
                OnPropertyChanged(nameof(Autologin));
            }
        }
        public string? Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string? Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public int ConnectionTimeout
        {
            get => connectionTimeout;
            set
            { 
                connectionTimeout = value;
                OnPropertyChanged(nameof(ConnectionTimeout));
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            this.execute(parameter);
        }
    }

    internal class MainViewModel : ViewModel
    {
        ObservableCollection<Page> pages = new ObservableCollection<Page>();
        ObservableCollection<Page> sidebarButtons = new ObservableCollection<Page>();
        ObservableCollection<Page> availablePages = new ObservableCollection<Page>();
        ObservableCollection<object> loadingTasks = new ObservableCollection<object>();
        public ObservableCollection<Page> AvailablePages { get => availablePages; }
        Page currentPage;
        Page mainPage;
        Page authPage;
        Page serverPage;
        Page adminPage;

        Page constructorPage;
        Page supplyPage;
        Page commissionPage;
        Page batchPage;
        Page commonPage;

        public Page AdminPage
        {
            get => adminPage;
            set 
            {
                adminPage = value;
                OnPropertyChanged(nameof(AdminPage));
            }
        }

        public Page CommonPage
        {
            get => commonPage;
            set
            {
                commonPage = value;
                OnPropertyChanged(nameof(AdminPage));
            }
        }

        public Page ConstructorPage
        {
            get => constructorPage;
            set
            {
                constructorPage = value;
                OnPropertyChanged(nameof(ConstructorPage));
            }
        }

        public Page SupplyPage
        {
            get=> supplyPage;
            set
            {
                supplyPage = value;
                OnPropertyChanged(nameof(SupplyPage));
            }
        }
        public Page CommissionPage
        {
            get => commissionPage;
            set
            {
                commissionPage = value;
                OnPropertyChanged(nameof(CommissionPage));
            }
        }

        public Page BatchPage
        {
            get => batchPage;
            set
            {
                batchPage = value;
                OnPropertyChanged(nameof(BatchPage));
            }
        }


        ServerListStatus serverListStatus = ServerListStatus.Updating;
        //ObservableCollection<(Server, Database[])> servers;
        bool isLoading;
        DataTable availableServers;

        UserSettings userSettings = new UserSettings();
        //UserSettings newSettings = new UserSettings();
        bool isInitialized;
        public bool IsInitialized
        {
            get => isInitialized;
            set
            {
                isInitialized = value;
                OnPropertyChanged(nameof(IsInitialized));
            }
        }
        UserType userType = UserType.Guest;

        // Json
        //public const JsonDocument settingsJson;

        // connections
        SqlConnection mainConnection;
        SqlConnection userConnection;

        HydropressDbContext mainContext;
        HydropressUserDbContext userContext;

        public HydropressDbContext MainContext
        {
            get => mainContext;
            set
            {
                mainContext = value;
                OnPropertyChanged(nameof(MainContext));
                OnContextChanged();
            }
        }

        public HydropressUserDbContext UserContext
        {
            get => userContext;
            set
            {
                userContext = value;
                OnPropertyChanged(nameof(UserContext));
            }
        }

        // time to connect to server
        int connectTimeout = 15;

        // selected db in server page
        private string selectedDb;

        // server page selected items
        private DataRowView selectedServer;

        // server page commands
        RelayCommand connectToServerCommand;
        RelayCommand updateServesCommand;
        RelayCommand setTextBoxToListBoxCommand;
        RelayCommand setTextBoxToDataGridCommand;

        // window commands
        RelayCommand closeWindowCommand;
        RelayCommand maximizeWindowCommand;
        RelayCommand minimizeWindowCommand;

        public MainViewModel()
        {
            #region Initialization
            mainPage = new MainMenu();
            authPage = new AuthorizationPage();
            serverPage = new ServerSelectorPage();

            // Соединение
            CurrentPage = serverPage;
            ReadJSONSettingsFile();
            var gotDbs = UserSettings.ServerName != null && UserSettings.MainDbName != null && UserSettings.UserDbName != null;
            if (gotDbs)
            {
                var connectTask = Task.Run(() => CreateConnectionToServer(UserSettings.ServerName, UserSettings.MainDbName, UserSettings.UserDbName));
                connectTask.Wait();
                if (connectTask.Result)
                {
                    CurrentPage = authPage;
                }
                if (UserSettings.Username != null && UserSettings.Password != null)
                {
                    var authTask = Task.Run(() => Authorize(UserSettings.Username, UserSettings.Password));
                }
            }
            else
            {
                UserSettings = new UserSettings();
                Task.Run(() => UpdateServers());
            }


            adminPage = new AdminPage();
            commonPage = new CommonPage();
            constructorPage = new ConstructorPage(MainContext);

            supplyPage = new SupplyPage(MainContext);
            commissionPage = new CommissionPage(MainContext);
            batchPage = new BatchPage(MainContext);


            pages.Add(serverPage);
            pages.Add(authPage);
            pages.Add(mainPage);
            //pages.Add();
            LoadingTasks.CollectionChanged += (s, e) => IsLoading = !loadingTasks.IsNullOrEmpty();
            Pages.CollectionChanged += (s, e) => { };
            AvailablePages.CollectionChanged += (s, e) => { };
            #endregion

        }

        #region Sidebar buttons bools

        bool isAuthorized = false;
        bool isCommissioner = false;
        bool isConstructor = false;
        bool isChief = false;

        public bool IsChief
        {
            get => isChief;
            set
            {
                isChief = value;
                OnPropertyChanged(nameof(IsChief));
            }
        }
        public bool IsConstructor
        {
            get => isConstructor;
            set
            {
                isConstructor = value;
                OnPropertyChanged(nameof(IsConstructor));
            }
        }
        public bool IsCommissioner
        {
            get => isCommissioner;
            set
            {
                isCommissioner = value;
                OnPropertyChanged(nameof(IsCommissioner));
            }
        }
        public bool IsAuthorized
        {
            get=> isAuthorized;
            set
            {
                isAuthorized = value;
                OnPropertyChanged(nameof(IsAuthorized));
            }
        }
        #endregion

        public async Task CreateJSONSettingsFile()
        {
            using (StreamWriter fs = new StreamWriter(new FileStream("settings.json", FileMode.Create)))
                await fs.WriteAsync(JsonConvert.SerializeObject(UserSettings, Formatting.Indented));
        }

        public void ReadJSONSettingsFile()
        {
            using (StreamReader fs = new StreamReader(new FileStream("settings.json", FileMode.OpenOrCreate)))
            {
                try
                {
                    UserSettings = JsonConvert.DeserializeObject<UserSettings>(fs.ReadToEnd())!;
                    UserSettings ??= new UserSettings();
                }
                catch (Exception e) {
                    MessageBox.Show(e.Message, "Ошибка в чтении файла настроек", MessageBoxButton.OK, MessageBoxImage.Error);
                    UserSettings = new UserSettings();
                }
                //TODO
            }
        }

        public void OnContextChanged()
        {
            if (constructorPage != null)
                ((ConstructorPage)constructorPage).MainContext = MainContext;
        }

        // statusbar strings
        //public string ServerName
        //{
        //    get => UserSettings.ServerName ?? "Server is not selected";
        //}
        //public string MainDatabaseName
        //{
        //    get => UserSettings.MainDbName ?? "Main DB is not selected";
        //}
        //public string UserDatabaseName
        //{
        //    get => UserSettings.UserDbName ?? "User DB is not selected";
        //}
        //public string UserName
        //{
        //    get => UserSettings.Username ?? "User is not authorized";
        //}

        public UserType UserType
        {
            get => userType;
            set
            {
                userType = value;
                IsChief = IsCommissioner = IsConstructor = false;
                OnPropertyChanged(nameof(UserType));
                switch ( value)
                {
                    case UserType.Admin:
                        IsChief = IsCommissioner = IsConstructor = true; break;
                    case UserType.Commissioner: IsCommissioner = true; break;
                    case UserType.Constructor: IsConstructor = true; break;
                    case UserType.ChiefWorker: IsChief = true; break;
                }
            }
        }
        int? _userId;
        public async Task<bool> Authorize(string username, string password)
        {
            IsAuthorized = false;
            var command = $@"SELECT * FROM [Users] WHERE Nickname = '{username}';";
            SqlDataAdapter adapter = new SqlDataAdapter(command, UserConnection);
            // Создаем объект Dataset
            DataSet ds = new DataSet();
            // Заполняем Dataset
            adapter.Fill(ds);
            ds.Dispose();
            adapter.Dispose();

            if (ds.Tables.Count < 1 && ds.Tables[0].Rows.Count < 1)
            {
                MessageBox.Show("Такого пользователя не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var row = ds.Tables[0].Rows[0];
            //if ((string)row[1] != username)
            //{   // Этого не должно быть
            //    MessageBox.Show("Такого пользователя не существует!","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return false;
            //}
            if ((string)row[2] != password)
            {
                MessageBox.Show("Неправильный пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            //REDO
            UserSettings.Username = username;
            if (UserSettings.Autologin)
            {
                UserSettings.Password = password;
            }
            else
            {
                UserSettings.Password = null;
            }
            

            _userId = (int)row[0];
            //TableAdapterManager tableAdapterManager = new TablfeAdapterManager();
            //TODO-REDO
            //UserContext.Users.Load();
            UserType = (UserType)(byte)row[3];//(UserType)(UserContext.Users.Find(_userId).Type);
            await CreateJSONSettingsFile();
            CurrentPage = mainPage;
            return IsAuthorized = true;
        }
        // server page commands
        public RelayCommand? ConnectToServerCommand
        {
            get
            {
                return connectToServerCommand ??
                    (connectToServerCommand = new RelayCommand(async obj =>
                    {
                        TextBox serverAdressTb = (TextBox)serverPage.FindName("serverAdressTb");
                        TextBox mainDbTb = (TextBox)serverPage.FindName("mainDbTb");
                        TextBox userDbTb = (TextBox)serverPage.FindName("userDbTb");
                        if (serverAdressTb.Text == "")
                        {
                            MessageBox.Show("Необходимо ввести сервер!","Внимание", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }
                        if (mainDbTb.Text == "")
                        {
                            MessageBox.Show("Необходимо ввести название основной базы данных!", "Внимание", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }
                        if (userDbTb.Text == "")
                        {
                            MessageBox.Show("Необходимо ввести название базы данных пользователей!", "Внимание", MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            return;
                        }

                        var res = CreateConnectionToServer(serverAdressTb.Text, mainDbTb.Text, userDbTb.Text);
                        await res;
                        if (res.Result)
                        {
                            UserSettings.ServerName = serverAdressTb.Text;
                            UserSettings.MainDbName = mainDbTb.Text;
                            UserSettings.UserDbName = userDbTb.Text;

                            CurrentPage = authPage;

                            //UserSettings = new UserSettings
                            //{
                            //    Autologin = false,
                            //    MainDbName = UserSettings.MainDbName,
                            //    UserDbName = UserSettings.UserDbName,
                            //    ServerName = ServerName,
                            //    Username = UserName,
                            //    Password = null,
                            //};

                            await CreateJSONSettingsFile();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось подключится к серверу.\r\nПроверьте правильность введённых данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            ClearStatusBarLabels();
                        }
                    }));
            }
        }

        void ClearStatusBarLabels()
        {
            UserSettings.ServerName = 
            UserSettings.MainDbName = 
            UserSettings.UserDbName = null;
        }

        public UserSettings UserSettings
        {
            get => userSettings;
            set
            {
                userSettings = value;
                OnPropertyChanged(nameof(UserSettings));
            }
        }

        public async Task<bool> CreateConnectionToServer(string server, string maindb, string userdb)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                Pooling = true,
                IntegratedSecurity = true,
                TrustServerCertificate = true,
                ConnectTimeout = UserSettings.ConnectionTimeout,
                DataSource = server,
                InitialCatalog = maindb,
            };

            MainConnection = new SqlConnection(builder.ConnectionString);
            builder.InitialCatalog = userdb;
            UserConnection = new SqlConnection(builder.ConnectionString);

            var res = ConnectToServer();
            LoadingTasks.Add(res);
            await res;
            LoadingTasks.Remove(res);
            if (res.Result == null)
            {
                Settings.Default.MainDbConnectionString = MainConnection.ConnectionString;
                Settings.Default.UserDbConnectionString = UserConnection.ConnectionString;

                MainContext = new HydropressDbContext(Settings.Default.MainDbConnectionString);
                UserContext = new HydropressUserDbContext(Settings.Default.UserDbConnectionString);
                UserContext.Users.Load();
                UserContext.UserMessages.Load();
                UserContext.UserMessagesContents.Load();
                IsInitialized = true;
            }

            return res.Result == null;

        }

        public async Task<Exception?> ConnectToServer()
        {
            try
            {
                var mainTask = MainConnection.OpenAsync();
                var userTask = UserConnection.OpenAsync();

                await mainTask;
                await userTask;

                if (mainTask.Exception != null && userTask != null) 
                    return new AggregateException(mainTask.Exception, userTask.Exception);
                if (mainTask != null && mainTask.Exception != null)
                    return mainTask.Exception;
                if (userTask != null && userTask.Exception != null)
                    return userTask.Exception;

                return null;
            }
            catch (Exception ex) { return ex; }
        }
        public async Task<Exception?> DisconectFromServer()
        {
            if (MainConnection == null && UserConnection == null)
                return new Exception("MainConnection == null || UserConnection == null");
            try
            {
                var mainTask = Task.Run(() => MainConnection.Close());
                var userTask = Task.Run(() => UserConnection.Close());

                await mainTask;
                await userTask;

                if (mainTask.Exception != null && userTask != null)
                    return new AggregateException(mainTask.Exception, userTask.Exception);
                if (mainTask != null && mainTask.Exception != null)
                    return mainTask.Exception;
                if (userTask != null && userTask.Exception != null)
                    return userTask.Exception;
                return null;
            }
            catch (Exception ex) { return ex; }
        }
        public RelayCommand? UpdateServesCommand
        {
            get
                {
                return updateServesCommand ??
                    (updateServesCommand = new RelayCommand(obj =>
                    {
                        Task.Run(() => UpdateServers());
                    }));
                }
        }
        public RelayCommand? SetTextBoxToListBoxCommand
        {
            get
                {
                return setTextBoxToListBoxCommand ??
                    (setTextBoxToListBoxCommand = new RelayCommand(obj =>
                    {
                        ((TextBox)obj).Text = SelectedDb;
                    }));
                }
        }
        public RelayCommand? SetTextBoxToDataGridCommand
        {
            get
            {
                return setTextBoxToDataGridCommand ??
                    (setTextBoxToDataGridCommand = new RelayCommand(obj =>
                    {
                        if (SelectedServer != null && SelectedServer[0] != null)
                            ((TextBox)obj).Text = SelectedServer == null ? "" : (string)SelectedServer[0];
                    }));
            }
        }

        // window commands
        public RelayCommand? CloseWindowCommand
        {
            get
            {
                return closeWindowCommand ??
                    (closeWindowCommand = new RelayCommand(obj =>Application.Current.Shutdown()));
            }
        }
        public RelayCommand? MaximizeWindowCommand
        {
            get
            {
                return maximizeWindowCommand ??
                    (maximizeWindowCommand = new RelayCommand(obj => {
                        if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Normal;
                        } 
                        else
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Maximized;
                        }
                    }));
            }
        }
        public RelayCommand? MinimizeWindowCommand
        {
            get
            {
                return minimizeWindowCommand ??
                    (minimizeWindowCommand = new RelayCommand(obj => Application.Current.MainWindow.WindowState =
                    Application.Current.MainWindow.WindowState = WindowState.Minimized));
            }
        }

        RelayCommand changeFrame;
        public RelayCommand? ChangeFrame
        {
            get
            {
                return changeFrame ??
                    (changeFrame = new RelayCommand(obj => {
                        var p = (Page)obj;
                        CurrentPage = p;
                    }));
            }
        }

        // connections
        public int ConnectionTimeout
        {
            get => connectTimeout;
            set
            {
                connectTimeout = value;
                OnPropertyChanged(nameof(ConnectionTimeout));
            }
        }
        public SqlConnection MainConnection
        {
            get => mainConnection;
            set
            {
                mainConnection = value;
                OnPropertyChanged(nameof(MainConnection));
                //Properties.Settings.Default.HydropressDBConnectionString = value.ConnectionString;
            }
        }
        public SqlConnection UserConnection
        {
            get => userConnection;
            set
            {
                userConnection = value;
                OnPropertyChanged(nameof(UserConnection));
                //Properties.Settings.Default.HydropressUserDBConnectionString = value.ConnectionString;
                
            }
        }
        public DataRowView SelectedServer
        {
            get => selectedServer;
            set
            {
                selectedServer = value;
                OnPropertyChanged(nameof(SelectedServer));
            }
        }
        
        public string SelectedDb
        {
            get => selectedDb;
            set
            {
                selectedDb = value;
                OnPropertyChanged(nameof(SelectedDb));
            }
        }
        public bool IsLoading
        {
            get => isLoading;
            private set
            {
                isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }
        public ServerListStatus ServerListStatus
        {
            get => serverListStatus;
            private set
            {
                serverListStatus = value;
                OnPropertyChanged("ServerListStatus");
            }
        }
        public Page MainPage
        {
            get => mainPage;
            set
            {
                mainPage = value;
                OnPropertyChanged(nameof(MainPage));
            }
        }
        public Page AuthPage
        {
            get => authPage;
            set
            {
                authPage = value;
                OnPropertyChanged(nameof(AuthPage));
            }
        }
        public Page ServerPage
        {
            get => serverPage;
            set
            {
                serverPage = value;
                OnPropertyChanged("ServerPage");
            }
        }
        public Page CurrentPage 
        { 
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }
        public DataTable AvailableServers { 
            get => availableServers; 
            private set
            {
                availableServers = value;
                //if (availableServers == null)
                //{
                //    ServerListStatus = ServerListStatus.Updating;
                //}
                //if (availableServers != null && availableServers.Rows.Count < 0)
                //{
                //    ServerListStatus = ServerListStatus.Updated;
                //}
                OnPropertyChanged("AvailableServers");
            }
        }
        public ObservableCollection<object> LoadingTasks { get => loadingTasks; }
        public ObservableCollection<Page> Pages { get => pages; }

        //
        //  Как включить:
        //  Запустить SQL Server Browser service (Служба обозревателя SQL Server)
        //  Это делается из SQL Server Manager (Диспетчер конфигурации SQL Server)
        //  Если не получается, то приложение ищет по реестрам только локальные сервера, иначе серверов нет.
        //
        public async Task UpdateServers()
        {
            LoadingTasks.Add("ServerList.Update");

            #region GetThroughRegedit
            // Дальнейший код является запасным вариантом нахождения локального сервера

            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Server");
            dt.Columns.Add("Instance");
            dt.Columns.Add("IsClustered").DataType = typeof(bool);
            dt.Columns.Add("Version");
            dt.Columns.Add("IsLocal").DataType = typeof(bool);
            var objRows = GetLocalSqlServerInstances2();
            for (int i = 0; i < objRows.Length; i++)
            {
                var row = dt.NewRow();
                row["Name"] = objRows[i][0];
                row["Server"] = objRows[i][1];
                row["Instance"] = objRows[i][2];
                row["IsClustered"] = objRows[i][3];
                row["Version"] = objRows[i][4];
                row["IsLocal"] = objRows[i][5];
                dt.Rows.Add(row);
            }

            if (dt.Rows.Count > 0)
            {
                AvailableServers = dt;
                ServerListStatus = ServerListStatus.Updated;
                LoadingTasks.Remove("ServerList.Update");
                return;
            }

            serverListStatus = ServerListStatus.Failed;
            #endregion
            #region GetNormally
            // Получить сервера; получить не только локальные сервера

            serverListStatus = ServerListStatus.Updating;
            var t = GetLocalSqlServerInstances();
            await t;

            if (t.Result.Rows.Count != 0)
            {
                AvailableServers = t.Result;
                ServerListStatus = ServerListStatus.Updated;
                LoadingTasks.Remove("ServerList.Update");
                return;
            }
            #endregion

            ServerListStatus = ServerListStatus.Failed;
            LoadingTasks.Remove("ServerList.Update");
        }
        async Task<DataTable> GetLocalSqlServerInstances()
        {
            var mainTask = Task.Run(()=>SmoApplication.EnumAvailableSqlServers());
            await mainTask;
            if (mainTask.IsCompleted && mainTask.Result.Rows.Count != 0)
            {
                return mainTask.Result;
            }
            return null;
        }
        private object[][] GetLocalSqlServerInstances2()
        {
            List<object[]> strings= new List<object[]>();
            string ServerName = Environment.MachineName;
            RegistryView registryView = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
            using (RegistryKey? hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            {
                RegistryKey? instanceKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                if (instanceKey != null)
                {
                    var valueNames = instanceKey.GetValueNames();
                    for (int i = 0; i < valueNames.Length; i++)
                    {
                        var value = instanceKey.GetValue(valueNames[i]) as string;
                        RegistryKey? serverPropsKey = hklm.OpenSubKey($@"SOFTWARE\Microsoft\Microsoft SQL Server\{value}", false);
                        strings.Add(
                            new object[]
                            {
                                ServerName + (valueNames[i] == "MSSQLSERVER" ? "" : "\\" + valueNames[i]),              // Name
                                ServerName,                                                                             // Server
                                (valueNames[i] == "MSSQLSERVER" ? "" : valueNames[i]),                                  // Instance
                                Convert.ToBoolean(                                                                      // | | |
                                    serverPropsKey?.OpenSubKey("ClusterState")?.GetValue("MPT_AGENT_CORE_CNI")) ||        // | | |
                                Convert.ToBoolean(                                                                      // ↓ ↓ ↓
                                    serverPropsKey?.OpenSubKey("ClusterState")?.GetValue("SQL_Engine_Core_Inst")),        // IsClustered
                                serverPropsKey?.OpenSubKey("MSSQLServer\\CurrentVersion")?.GetValue("CurrentVersion")!,    // Version
                                true,                                                                                   // IsLocal
                            }
                        );
                    }
                }
            }
            return strings.ToArray();
        }
    }
}
