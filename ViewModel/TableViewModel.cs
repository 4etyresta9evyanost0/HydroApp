using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroApp
{
    class TableViewModel : ViewModel
    {
        #region Fields
        HydropressDbContext _mainDb = new HydropressDbContext();
        HydropressUserDbContext _userDb = new HydropressUserDbContext();


        DbContext[] _dbs;
        #endregion

        #region Properties
        public HydropressDbContext MainDb { get => _mainDb; }
        public HydropressUserDbContext UserDb { get => _userDb; }
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
            _dbs = new DbContext[] { _mainDb, _userDb };
            System.Windows.Application.Current.MainWindow.Closing += DisposeOfContexts;
            foreach (var db in _dbs)
                db?.Database.EnsureCreated(); // гарантируем, что база данных создана
        }
    }
}
