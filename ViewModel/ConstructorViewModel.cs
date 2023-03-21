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
    internal class ConstructorViewModel : ViewModel
    {
        public HydropressDbContext Context { get; set; }

        ListBox _designerLb;
        ListBox _detailLb;
        ListBox _productLb;

        ComboBox _detailDeveloperTb;

        public ComboBox DetailDeveloperTb
        {
            get => _detailDeveloperTb;
            set
            {
                _detailDeveloperTb = value;
                OnPropertyChanged(nameof(DetailDeveloperTb));
            }
        }


        public ListBox DesignerLb{
            get => _designerLb;
            set {
                _designerLb = value;
                OnPropertyChanged(nameof(DesignerLb));
            }
        }

        public ListBox DetailLb {
            get => _detailLb ;
            set{
                _detailLb  = value;
                OnPropertyChanged(nameof(DetailLb));
            }
        }

        public ListBox ProductLb{
            get=> _productLb;
            set{
                _productLb = value;
                OnPropertyChanged(nameof(ProductLb));
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


            Context.Constructions.Load();

            Context.Details.Load();
            Context.Materials.Load();
            Context.MaterialsForDetails.Load();

            Context.Productions.Load();

          

            Designers = Context.Designers.Local.ToObservableCollection();
            Designers.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Designers));

            Details = Context.Details.Local.ToObservableCollection();
            Details.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Details));

            Products = Context.Productions.Local.ToObservableCollection();
            Products.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Products));




            DetailDeveloperTb.ItemsSource = Designers;
        }

        public ConstructorViewModel(HydropressDbContext context)
        {
            Context = context;
            Context.SaveChangesFailed += (x, e) =>
            {
                MessageBox.Show(e.Exception.Message, "Ошибка");
            };
            Context.SavedChanges += (x, e) =>
            {
                OnPropertyChanged(nameof(Designers));
                OnPropertyChanged(nameof(Details));
                OnPropertyChanged(nameof(Products));
            };
        }


        public RelayCommand _updateCommand;
        public RelayCommand UpdateCommand
        {
            get => _updateCommand ??= new RelayCommand(obj =>
            {
                
            });
        }


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


        #region Designer 

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

        Employee? _newEmployee;
        Designer? _newDesigner;

        public Designer? NewDesigner
        {
            get => _newDesigner;
            set
            {
                _newDesigner = value;
                OnPropertyChanged(nameof(NewDesigner));
            }
        }

        RelayCommand _addCommandDesigner;
        public RelayCommand AddCommandDesigner {
            get{
                return _addCommandDesigner ??= new RelayCommand(obj =>
                {
                    _newEmployee = new Employee();
                    NewDesigner = new Designer
                    {
                        IdNavigation = _newEmployee
                    };
                    Context.Employees.Add(_newEmployee);
                    Context.Designers.Add(NewDesigner);

                    SelectedValueDesigner = Designers.Count - 1;
                });
            }    
        }

        RelayCommand _saveCommandDesigner;
        public RelayCommand SaveCommandDesigner
        {
            get
            {
                return _saveCommandDesigner ??= new RelayCommand(obj =>
                {
                    int amountAdded;
                    string error = "";

                    try
                    {
                        amountAdded = Context.SaveChanges();
                        Designers = Context.Designers.Local.ToObservableCollection();
                        Designers.CollectionChanged += (x, ev) => OnPropertyChanged(nameof(Details));
                        DesignerLb.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        amountAdded = -1;
                        error = ex.Message;
                    }
                    MessageBox.Show($"Код: {(amountAdded == -1 ? $"-1\r\nПроизошла ошибка: {error}" : (amountAdded == 0 ? "0\r\nЗаписи не были добавлены" : $"1\r\nКоличество добавленных/изменённых записей: {amountAdded}"))}");
                });
            }
        }


        RelayCommand _removeCommandDesigner;
        public RelayCommand RemoveCommandDesigner
        {
            get
            {
                return _removeCommandDesigner ??= new RelayCommand(obj =>
                {
                    // TODO: добавить возможность убрать окно предупреждения
                    // TODO: добавить ограничение только для админов
                    if (MessageBox.Show("Вы уверены, что хотите удалить выделенный элемент?", "") == MessageBoxResult.OK)
                    {
                        Designers.Remove(SelectedDesigner);
                    }
                });
            }
        }
        #endregion

        #region Detail 

        ObservableCollection<Material> _materials;

        public ObservableCollection<Material> Materials
        {
            get => _materials;
            set
            {
                _materials = value;
                OnPropertyChanged(nameof(Materials));
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

        Construction? _newConstructionDetail;
        Detail? _newDetail;

        public Detail? NewDetail
        {
            get => _newDetail;
            set
            {
                _newDetail = value;
                OnPropertyChanged(nameof(NewDetail));
            }
        }

        RelayCommand _addCommandDetail;
        public RelayCommand AddCommandDetail
        {
            get
            {
                return _addCommandDetail ??= new RelayCommand(obj =>
                {
                    _newConstructionDetail = new Construction();
                    NewDetail = new Detail
                    {
                        IdNavigation = _newConstructionDetail
                    };
                    Context.Employees.Add(_newEmployee);
                    Context.Details.Add(NewDetail);


                });
            }
        }

        RelayCommand _saveCommandDetail;
        public RelayCommand SaveCommandDetail
        {
            get
            {
                return _saveCommandDetail ??= new RelayCommand(obj =>
                {
                    int amountAdded;
                    string error = "";
                    
                    try
                    {
                        amountAdded = Context.SaveChanges();
                        Details = Context.Details.Local.ToObservableCollection();
                        Details.CollectionChanged += (x,ev)=> OnPropertyChanged(nameof(Details));
                        DetailLb.Items.Refresh();
                    }
                    catch (Exception ex)
                    {
                        amountAdded = -1;
                        error = ex.Message;
                    }
                    MessageBox.Show($"Код: {(amountAdded == -1? $"-1\r\nПроизошла ошибка: {error}" : (amountAdded == 0 ? "0\r\nЗапеси не были добавлены" : $"1\r\nКоличество добавленных записей:{amountAdded}"))}");
                });
            }
        }


        RelayCommand _removeCommandDetail;
        public RelayCommand RemoveCommandDetail
        {
            get
            {
                return _removeCommandDetail ??= new RelayCommand(obj =>
                {
                    MessageBox.Show("Удаление детали");
                });
            }
        }
        #endregion

        #region Product 

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

        Construction? _newConstructionProduction;
        Production? _newProduct;

        public Production? NewProduct
        {
            get => _newProduct;
            set
            {
                _newProduct = value;
                OnPropertyChanged(nameof(NewProduct));
            }
        }

        RelayCommand _addCommandProduct;
        public RelayCommand AddCommandProduct
        {
            get
            {
                return _addCommandProduct ??= new RelayCommand(obj =>
                {
                    _newConstructionProduction = new Construction();
                    NewProduct = new Production
                    {
                        IdNavigation = _newConstructionProduction
                    };
                    Context.Employees.Add(_newEmployee);
                    Context.Productions.Add(NewProduct);


                });
            }
        }

        RelayCommand _saveCommandProduct;
        public RelayCommand SaveCommandProduct
        {
            get
            {
                return _saveCommandProduct ??= new RelayCommand(obj =>
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


        RelayCommand _removeCommandProduct;
        public RelayCommand RemoveCommandProduct
        {
            get
            {
                return _removeCommandProduct ??= new RelayCommand(obj =>
                {
                    MessageBox.Show("Удаление продукции");
                });
            }
        }
        #endregion
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
