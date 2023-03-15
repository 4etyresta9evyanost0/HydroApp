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
using System.Windows.Data;

namespace HydroApp
{
    internal class ConstructorViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }

        ObservableCollection<Designer> _designers;
        public ObservableCollection<Designer> Designers
        {
            get => _designers;
            set
            {
                _designers = value;
                OnPropertyChanged(nameof(Designers));
            }
        }

        ObservableCollection<Detail> _details;
        public ObservableCollection<Detail> Details
        {
            get => _details;
            set
            {
                _details = value;
                OnPropertyChanged(nameof(Details));
            }
        }

        ObservableCollection<Production> _products;
        public ObservableCollection<Production> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }


        private int selectedValueDesigner;
        public int SelectedValueDesigner
        {
            get { return selectedValueDesigner; }
            set { selectedValueDesigner = value;
                OnPropertyChanged(nameof(SelectedValueDesigner));
            }
        }

        private int selectedValueDetail;
        public int SelectedValueDetail
        {
            get { return selectedValueDetail; }
            set
            {
                selectedValueDetail = value;
                OnPropertyChanged(nameof(SelectedValueDetail));
            }
        }

        private int selectedValueProduct;
        public int SelectedValueProduct
        {
            get { return selectedValueProduct; }
            set
            {
                selectedValueProduct = value;
                OnPropertyChanged(nameof(SelectedValueProduct));
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

        public Detail? selectedDetail;
        public Detail? SelectedDetail
        {
            get => selectedDetail;
            set
            {
                selectedDetail = value;
                OnPropertyChanged(nameof(SelectedDetail));
            }
        }
        public Production? selectedProduct;
        public Production? SelectedProduct
        {
            get => selectedProduct;
            set
            {
                selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
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

        Employee? _newEmployee;
        Designer? _newDesigner;


        // TODO: не используется
        bool _isAddingNew;
        public bool IsAddingNew
        {
            get => _isAddingNew;
            set
            {
                _isAddingNew = value;
                OnPropertyChanged(nameof(IsAddingNew));
            }
        }


        public Designer? NewDesigner
        {
            get => _newDesigner;
            set
            {
                _newDesigner = value;
                OnPropertyChanged(nameof(NewDesigner));
            }
        }

        RelayCommand _addCommand;
        public RelayCommand AddCommand {
            get{
                return _addCommand ??= new RelayCommand(obj =>
                {
                    _newEmployee = new Employee();
                    NewDesigner = new Designer
                    {
                        IdNavigation = _newEmployee
                    };
                    Context.Employees.Add(_newEmployee);
                    Context.Designers.Add(NewDesigner);

                    
                });
            }    
        }

        RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(obj =>
                {
                    int id;
                    try
                    {
                        id = Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        id = -1;
                    }
                    MessageBox.Show($"Код: {id}");
                });
            }
        }


        RelayCommand _removeCommand;
        public RelayCommand RemoveCommand
        {
            get
            {
                return _removeCommand ??= new RelayCommand(obj =>
                {
                    MessageBox.Show("b");
                });
            }
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
