using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HydroApp
{
    class SupplyViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }

        public SupplyViewModel(HydropressDbContext context)
        {
            Context = context;
            Context.SaveChangesFailed += (x, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Ошибка");
            };
            Context.SavedChanges += (x, e) =>
            {
                //OnPropertyChanged(nameof(Designers));
                //OnPropertyChanged(nameof(Details));
                //OnPropertyChanged(nameof(Products));
            };
        }

        public void Init()
        {
            Context.Suppliers.Load();
            Context.Supplies.Load();
            Context.SupplyDetails.Load();
        }

    }
}
