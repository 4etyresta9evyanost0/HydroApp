﻿using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace HydroApp
{
    internal class AdminPageViewModel : ViewModel
    {
        ObservableCollection<string> tables = new ObservableCollection<string>();

        public List<string> MainDBTables = new List<string> {
            "CommissionDetails",
            "Employees",
            "Workshops",
            "Foremen",
            "Designers",
            "Constructions",
            "Productions",
            "Materials",
            "Clients",
            "Commissions",
            "Suppliers",
            "Supplies",
            "SupplyDetails",
            "Details",
            "DetailsForProductions",
            "Batch",
            "MaterialsForDetails",
        };


        public List<string> UserDBTables = new List<string>{
            "Users",
            "User_Messages",
            "User_Messages_Content",
        };

        private object selectedTable;
        private object selectedDatabase;

        public AdminPageViewModel()
        {
        }

        public object SelectedTable
        {
            get => selectedTable;
            set
            {
                selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
            }
        }

        public object SelectedDatabase
        {
            get => selectedDatabase;
            set
            {
                selectedDatabase = value;
                OnPropertyChanged(nameof(SelectedDatabase));
            }
        }

        public SqlConnection SelectedConnection { get => (string)SelectedDatabase == "MainDB" ? Mvm.MainConnection : Mvm.UserConnection; }

        DataSet dataSet;
        public DataSet DataSet
        {
            get => dataSet;
            set
            {
                dataSet = value;
                OnPropertyChanged(nameof(DataSet));
            }
        }

        SqlDataAdapter sqlDataAdapter;
        public SqlDataAdapter SqlDataAdapter
        {
            get => sqlDataAdapter;
            set
            {
                sqlDataAdapter = value;
                OnPropertyChanged(nameof(SqlDataAdapter));
            }
        }

        RelayCommand databaseChange;
        public RelayCommand DatabaseChange
        {
            get => databaseChange ?? (databaseChange = new RelayCommand(obj =>
            {
                var selected = (string)obj;
                List<string> selectedList;
                Tables.Clear();
                selectedList = selected == "MainDB" ? MainDBTables : UserDBTables;
                selectedList.ForEach(x => Tables.Add(x));
            }
            ));
        }

        RelayCommand tableChange;
        public RelayCommand TableChange
        {
            get => tableChange ?? (tableChange = new RelayCommand(obj =>
            {
                if (obj == null) return;
                var selected = (string)obj;
                var command = $@"SELECT * FROM [{obj}]";
                if (SqlDataAdapter != null) SqlDataAdapter.Dispose();
                SqlDataAdapter = new SqlDataAdapter(command, SelectedConnection);
                commandBuilder = new SqlCommandBuilder(SqlDataAdapter);
                if (DataSet != null) DataSet.Dispose();
                DataSet ds = new DataSet();
                SqlDataAdapter.Fill(ds);
                //if (DataSet != null) DataSet.Dispose();
                DataSet = ds;
            }
            ));
        }
        RelayCommand saveChanges;

        SqlCommandBuilder commandBuilder;

        public RelayCommand SaveChanges
        {
            get => saveChanges ?? (saveChanges = new RelayCommand(obj =>
            {
                try
                {
                    if (DataSet != null)
                    {
                        SqlDataAdapter.Update(DataSet);
                        DataSet.Clear();
                        SqlDataAdapter.Fill(DataSet);
                        MessageBox.Show("База данных обновлена", "Внимание");
                    }
                    else
                    {
                        MessageBox.Show("Выберите базу данных и таблицу", "Ошибка");
                    }
                }
                catch (Exception e) 
                {
                    MessageBox.Show(e.Message, "Ошибка");
                }
            }));
        }

        public ObservableCollection<string> Tables { get { return tables; } }
        public MainViewModel Mvm { get => (MainViewModel)Application.Current.Resources["Mvm"]; }
    }
}
