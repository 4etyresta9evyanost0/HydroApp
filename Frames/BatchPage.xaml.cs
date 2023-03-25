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
    /// Логика взаимодействия для BatchPage.xaml
    /// </summary>
    public partial class BatchPage : Page
    {
        TableViewModel TableViewModel { get => (TableViewModel)Application.Current.Resources["tableVm"]; }
        BatchViewModel BatchVm { get => TableViewModel.BatchVm; }
        public HydropressDbContext MainContext;
        public BatchPage(HydropressDbContext context)
        {
            MainContext = context;
            InitializeComponent();
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BatchVm.Init();
            Binding b = new Binding();
            b.Source = BatchVm;
            b.Path = new PropertyPath("Details");
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _detailCb.SetBinding(ComboBox.ItemsSourceProperty, b);
            b = new Binding();
            b.Source = BatchVm;
            b.Path = new PropertyPath("Foremen");
            b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            _foremanCb.SetBinding(ComboBox.ItemsSourceProperty, b);
        }

    }
}
