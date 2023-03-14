using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для ConstructorPage.xaml
    /// </summary>
    public partial class ConstructorPage : Page
    {
        TableViewModel TableViewModel { get => (TableViewModel)Application.Current.Resources["tableVm"]; }
        public HydropressDbContext MainContext;



        public ObservableCollection<Designer> Designers;

        public RelayCommand? SaveCommand { get; }
        public RelayCommand? DeleteCommand { get; }
        public RelayCommand? AddCommand { get; }

        public ConstructorPage(HydropressDbContext context)
        {
            MainContext = context; //= new HydropressDbContext(Settings.Default.MainDbConnectionString);
            InitializeComponent();
            //TableViewModel = (DataContext as TableViewModel);
            Loaded += Page_Loaded;
        }

        private void DesignerSelected(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.Init();
        }

        private void DetailSelected(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void ProductionSelected(object sender, RoutedEventArgs e)
        {
            ;
        }


        //// при загрузке окна
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            //TableViewModel?.MainDb.Database.EnsureCreated();
            MainContext.Database.EnsureCreated();

            // Designer
            MainContext.Designers.Load();
            MainContext.Employees.Load();

            Designers = MainContext.Designers.Local.ToObservableCollection();
            //_allDesigners.ItemsSource = MainContext.Designers.Local;
            //_desginerTabItem.DataContext = MainContext.Designers;

            // 

            // загружаем данные из БД
            //TableViewModel.MainDb.Designers.Load();
            // и устанавливаем данные в качестве контекста
        }
    }
}
