using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroApp
{
    internal class ConstructorViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }
        public ObservableCollection<Designer>? Designers { get; set; }

        public Designer? selectedDesigner;
        public Designer? SelectedDesigner
        {
            get => selectedDesigner;
            set 
            {
                selectedDesigner = value;
                OnPropertyChanged(nameof(SelectedDesigner));
            }
        }


        public ConstructorViewModel(HydropressDbContext context)
        {
            Context = context;
            Context.Database.EnsureCreated();
            Context.Designers.Load();
            Context.Employees.Load();

            Designers = Context.Designers.Local.ToObservableCollection();
        }
    }
}
