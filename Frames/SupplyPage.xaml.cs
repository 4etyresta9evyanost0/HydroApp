using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для SupplyPage.xaml
    /// </summary>
    public partial class SupplyPage : Page
    {
        TableViewModel TableViewModel { get => (TableViewModel)Application.Current.Resources["tableVm"]; }
        SupplyViewModel SupplyVm { get => TableViewModel.SupplyVm; }
        public HydropressDbContext MainContext;
        public SupplyPage(HydropressDbContext context)
        {
            MainContext = context;
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SupplyVm.Init();
            Binding b = new Binding();
            b.Source = SupplyVm;
            b.Path = new PropertyPath("Suppliers");
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _supplierCb.SetBinding(ComboBox.ItemsSourceProperty, b);
        }

        private void OpenAddingMaterialsMenu(object sender, RoutedEventArgs e)
        {
            var window = new AddNewMaterial(MainContext, (IEnumerable<SupplyDetail>)_MtMLb.ItemsSource);
            if (window.ShowDialog() == true)
            {
                var res = window.Result;

                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].Item1 && res[i].Item3 > 0)
                    {
                        var sd = new SupplyDetail
                        {
                            IdSupply = SupplyVm.SelectedItem.Id,
                            IdMaterial = res[i].Item2.Id,
                            MaterialAmount = res[i].Item3
                        };
                        SupplyVm.SelectedItem.SupplyDetails.Add(sd);
                    }
                }
                TableViewModel.ConstrVm.Init();
                RefreshMaterials();
            }
        }

        void RefreshMaterials()
        {
            var temp = SupplyVm.SelectedIndex;
            SupplyVm.SelectedIndex = 0;
            _MtMLb.Items.Refresh();
            SupplyVm.SelectedIndex = temp;
        }

        private void DeleteMaterailForDetail(object sender, RoutedEventArgs e)
        {
            SupplyVm.SelectedItem.SupplyDetails.Remove(SupplyVm.SelectedSupplyDetail);
            RefreshMaterials();
            //MainContext.MaterialsForDetails.Remove(TableViewModel.ConstrVm.SelectedMaterailForDetail);
        }
    }
}
