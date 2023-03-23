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
using System.Windows.Media.Media3D;
using Xceed.Wpf.AvalonDock.Controls;

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

        public ConstructorPage(HydropressDbContext context)
        {
            MainContext = context; //= new HydropressDbContext(Settings.Default.MainDbConnectionString);
            InitializeComponent();
            //TableViewModel = (DataContext as TableViewModel);

            var b = new Binding();
            b.Source = TableViewModel.ConstrVm;
            b.Path = new PropertyPath("SelectedMaterailForDetail");
            b.Mode = BindingMode.TwoWay;
            _allMaterials.SetBinding(ListBox.SelectedItemProperty, b);

            b = new Binding();
            b.Source = TableViewModel.ConstrVm;
            b.Path = new PropertyPath("SelectedDetailForProduction");
            b.Mode = BindingMode.TwoWay;
            _allDetailsForProduction.SetBinding(ListBox.SelectedItemProperty, b);

            Loaded += Page_Loaded;
            TableViewModel.ConstrVm.DesignerLb = _allDesigners;
            TableViewModel.ConstrVm.DetailLb = _allDetails;
            TableViewModel.ConstrVm.ProductLb = _allProducts;

            TableViewModel.ConstrVm.DetailDeveloperTb = _detailDeveloperTb;
            TableViewModel.ConstrVm.ProductDeveloperTb = _productDeveloperTb;
        }

        private void DesignerSelected(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.Init();
        }

        private void DetailSelected(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.Init(); ;
        }

        private void ProductionSelected(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.Init(); ;
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
        }

        private void OpenAddingMaterialsMenu(object sender, RoutedEventArgs e)
        {
            var window = new AddNewMaterial(MainContext, (IEnumerable<MaterialsForDetail>)_allMaterials.ItemsSource);
            if (window.ShowDialog() == true)
            {
                var res = window.Result;

                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].Item1 && res[i].Item3 > 0)
                    {
                        var mfd = new MaterialsForDetail
                        {
                            //IdDetailNavigation = TableViewModel.ConstrVm.SelectedDetail,
                            //IdMaterialNavigation = res[i].Item2,
                            IdDetail = TableViewModel.ConstrVm.SelectedDetail.Id,
                            IdMaterial = res[i].Item2.Id,
                            MaterialsAmount = res[i].Item3
                        };

                        //MainContext.MaterialsForDetails.Add(mfd);
                        TableViewModel.ConstrVm.SelectedDetail.MaterialsForDetails.Add(mfd);
                        //mfd.IdDetailNavigation = TableViewModel.ConstrVm.SelectedDetail;
                        //mfd.IdMaterialNavigation = res[i].Item2;
                    }
                }
                //MainContext.SaveChanges();
                //MainContext.MaterialsForDetails.Load();
                //MainContext.Details.Load();
                TableViewModel.ConstrVm.Init();
                RefreshMaterials();
            }
        }

        void RefreshMaterials()
        {
            var temp = TableViewModel.ConstrVm.SelectedValueDetail;
            TableViewModel.ConstrVm.SelectedValueDetail = 0;
            _allMaterials.Items.Refresh();
            TableViewModel.ConstrVm.SelectedValueDetail = temp;
        }

        private void DeleteMaterailForDetail(object sender, RoutedEventArgs e)
        {
            //MenuItem mi = sender as MenuItem;
            //ContextMenu cm = mi.CommandParameter as ContextMenu;
            //StackPanel g = cm.PlacementTarget as StackPanel;
            //var c = VisualTreeHelper.GetParent(g).FindLogicalAncestor<ListBoxItem>();
            TableViewModel.ConstrVm.SelectedDetail.MaterialsForDetails.Remove(TableViewModel.ConstrVm.SelectedMaterailForDetail);
            RefreshMaterials();
            //MainContext.MaterialsForDetails.Remove(TableViewModel.ConstrVm.SelectedMaterailForDetail);
        }

        private void OpenAddingDetailsForProductMenu(object sender, RoutedEventArgs e)
        {
            var window = new AddNewDetail(MainContext, (IEnumerable<DetailsForProduction>)_allDetailsForProduction.ItemsSource);
            if (window.ShowDialog() == true)
            {
                var res = window.Result;

                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].Item1 && res[i].Item3 > 0)
                    {
                        var dfp = new DetailsForProduction
                        {
                            //IdDetailNavigation = TableViewModel.ConstrVm.SelectedDetail,
                            //IdMaterialNavigation = res[i].Item2,
                            IdProduction = TableViewModel.ConstrVm.SelectedProduct.Id,
                            IdDetail = res[i].Item2.Id,
                            DetailsAmount = res[i].Item3
                        };

                        //MainContext.MaterialsForDetails.Add(mfd);
                        TableViewModel.ConstrVm.SelectedProduct.DetailsForProductions.Add(dfp);
                        //mfd.IdDetailNavigation = TableViewModel.ConstrVm.SelectedDetail;
                        //mfd.IdMaterialNavigation = res[i].Item2;
                    }
                }
                //MainContext.SaveChanges();
                //MainContext.MaterialsForDetails.Load();
                //MainContext.Details.Load();
                TableViewModel.ConstrVm.Init();
                RefreshDetails();
            }
        }

        void RefreshDetails()
        {
            var temp = TableViewModel.ConstrVm.SelectedValueProduct;
            TableViewModel.ConstrVm.SelectedValueProduct = 0;
            _allDetailsForProduction.Items.Refresh();
            TableViewModel.ConstrVm.SelectedValueProduct = temp;
        }

        private void DeleteDetailForProduction(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.SelectedProduct.DetailsForProductions.Remove(TableViewModel.ConstrVm.SelectedDetailForProduction);
            RefreshDetails();
        }
    }
}
