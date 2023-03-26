using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace HydroApp
{
    class TableViewModel : ViewModel
    {
        #region Fields
        HydropressDbContext _mainDb;
        HydropressUserDbContext _userDb;

        ConstructorViewModel constrVm;
        SupplyViewModel supplyVm;
        CommissionViewModel commissVm;
        BatchViewModel batchVm;
        CommonPageViewModel commonPageVm;

        List<string> _mainDBTables = new List<string> {
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
        List<string> _userDBTables = new List<string>{
            "Users",
            "User_Messages",
            "User_Messages_Content",
        };

        //public Designer? Designer { get; }
        //public Detail? Detail { get; }
        //public Production? Production { get; }

        DbContext[] _dbs;
        #endregion

        #region Properties
        public HydropressDbContext MainDb 
        {   get => _mainDb; 
            set 
            { 
                _mainDb = value; 
                OnPropertyChanged(nameof(MainDb));
                OnContextChanged();
            } 
        }
        public HydropressUserDbContext UserDb
        { 
            get => _userDb; 
            set 
            { 
                _userDb = value; 
                OnPropertyChanged(nameof(UserDb)); 
            } 
        }

        public ConstructorViewModel ConstrVm
        {
            get => constrVm;
            set
            {
                constrVm = value;
                OnPropertyChanged(nameof(ConstrVm));
            }
        }
        public SupplyViewModel SupplyVm
        {
            get => supplyVm;
            set
            {
                supplyVm = value;
                OnPropertyChanged(nameof(SupplyVm));
            }
        }
        public CommissionViewModel CommissVm
        {
            get => commissVm;
            set
            {
                commissVm = value;
                OnPropertyChanged(nameof(CommissVm));
            }
        }
        public BatchViewModel BatchVm
        {
            get => batchVm;
            set
            {
                batchVm = value;
                OnPropertyChanged(nameof(BatchVm));
            }
        }
        public CommonPageViewModel CommonPageVm
        {
            get => commonPageVm;
            set
            {
                commonPageVm = value;
                OnPropertyChanged(nameof(CommonPageVm));
            }
        }

        #endregion


        #region Methods

        async void DisposeOfContexts(object? sender, CancelEventArgs e)
        {
            var maint = MainDb.DisposeAsync();
            var usert = UserDb.DisposeAsync();
            await maint;
            await usert;
        }

        void LoadEvery()
        {
            foreach (var db in _dbs)
                db?.Database.EnsureCreated(); // гарантируем, что база данных создана

            MainDb.Employees.Load();
            MainDb.Batches.Load();
            MainDb.Clients.Load();
            MainDb.Commissions.Load();
            MainDb.CommissionDetails.Load();
            MainDb.Constructions.Load();
            MainDb.Designers.Load();
            MainDb.Details.Load();
            MainDb.DetailsForProductions.Load();
            MainDb.Foremen.Load();
            MainDb.Materials.Load();
            MainDb.MaterialsForDetails.Load();
            MainDb.Productions.Load();
            MainDb.Suppliers.Load();
            MainDb.Supplies.Load();
            MainDb.SupplyDetails.Load();
            MainDb.Workshops.Load();
        }

        public void OnContextChanged()
        {
            if (ConstrVm != null)
                ConstrVm.Context = MainDb;
        }
        #endregion

        public TableViewModel()
        {
            MainDb = new HydropressDbContext(Settings.Default.MainDbConnectionString);
            UserDb = new HydropressUserDbContext(Settings.Default.UserDbConnectionString);
            constrVm = new ConstructorViewModel(_mainDb);
            supplyVm = new SupplyViewModel(_mainDb);
            commissVm = new CommissionViewModel(_mainDb);
            batchVm = new BatchViewModel(_mainDb);
            commonPageVm = new CommonPageViewModel(_mainDb);


            _dbs = new DbContext[] { _mainDb, _userDb };
            System.Windows.Application.Current.MainWindow.Closing += DisposeOfContexts;

        }
    }

    public class CollectionToIEnumerableObject : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable en)
            {
                var b = en.Cast<object>();
                return new ObservableCollection<object>(b);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
