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
using System.Windows.Data;

namespace HydroApp
{
    internal class ConstructorViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }

        public ObservableCollection<Designer> _designers;
        public ObservableCollection<Designer> Designers 
        { 
            get => _designers;
            set 
            {
                _designers = value;
                OnPropertyChanged(nameof(Designers));
            } 
        }

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

        public void Init()
        {
            Context.Database.EnsureCreated();
            Context.Designers.Load();
            Context.Employees.Load();
            Designers = Context.Designers.Local.ToObservableCollection();
            Designers.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Designers));
        }

        public ConstructorViewModel(HydropressDbContext context)
        {
            Context = context;
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
