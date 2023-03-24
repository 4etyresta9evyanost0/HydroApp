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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для CommissionPage.xaml
    /// </summary>
    public partial class CommissionPage : Page
    {
        TableViewModel TableViewModel { get => (TableViewModel)Application.Current.Resources["tableVm"]; }
        CommissionViewModel CommissVm { get => TableViewModel.CommissVm; }
        public HydropressDbContext MainContext;
        public CommissionPage(HydropressDbContext context)
        {
            MainContext = context;
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CommissVm.Init();
            Binding b = new Binding();
            b.Source = CommissVm;
            b.Path = new PropertyPath("Clients");
            _clientCb.SetBinding(ComboBox.ItemsSourceProperty, b);
        }

        private void DeleteConstructionForCommission(object sender, RoutedEventArgs e)
        {
            CommissVm.SelectedItem.CommissionDetails.Remove(CommissVm.SelectedCommissionDetail);
            RefreshConstructions();
        }

        private void OpenAddingConstructionForCommissionMenu(object sender, RoutedEventArgs e)
        {
            var window = new AddNewConstruction(MainContext, (IEnumerable<CommissionDetail>)_MtMLb.ItemsSource);
            if (window.ShowDialog() == true)
            {
                var res = window.Result;

                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].Item1 && res[i].Item3 > 0)
                    {
                        var cd = new CommissionDetail
                        {
                            IdCommission = CommissVm.SelectedItem.Id,
                            IdConstruction = res[i].Item2.Id,
                            ConstructionsAmount = res[i].Item3
                        };
                        CommissVm.SelectedItem.CommissionDetails.Add(cd);
                    }
                }
                CommissVm.Init();
                RefreshConstructions();
            }
        }

        void RefreshConstructions()
        {
            var temp = CommissVm.SelectedIndex;
            CommissVm.SelectedIndex = 0;
            _MtMLb.Items.Refresh();
            CommissVm.SelectedIndex = temp;
        }

    }
}
