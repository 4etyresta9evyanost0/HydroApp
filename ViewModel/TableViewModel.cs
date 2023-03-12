using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace HydroApp
{
    class TableViewModel : ViewModel
    {
        #region Fields
        HydropressDbContext _mainDb = new HydropressDbContext();
        HydropressUserDbContext _userDb = new HydropressUserDbContext();

        ConstructorViewModel constrVm;
        DataPanelViewModel dataPanelVm;

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
        public HydropressDbContext MainDb { get => _mainDb; }
        public HydropressUserDbContext UserDb { get => _userDb; }

        public DataPanelViewModel DataPanelVm
        {
            get => dataPanelVm;
            set
            {
                dataPanelVm = value;
                OnPropertyChanged(nameof(DataPanelVm));
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
        #endregion


        #region Methods

        async void DisposeOfContexts(object? sender, CancelEventArgs e)
        {
            var maint = MainDb.DisposeAsync();
            var usert = UserDb.DisposeAsync();
            await maint;
            await usert;
        }
        #endregion

        public TableViewModel()
        {
            dataPanelVm = new DataPanelViewModel();
            constrVm = new ConstructorViewModel(_mainDb);
            _dbs = new DbContext[] { _mainDb, _userDb };
            System.Windows.Application.Current.MainWindow.Closing += DisposeOfContexts;
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
    }
}
