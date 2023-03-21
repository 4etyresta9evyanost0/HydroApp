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
            Loaded += Page_Loaded;
            TableViewModel.ConstrVm.DesignerLb = _allDesigners;
            TableViewModel.ConstrVm.DetailLb = _allDetails;
            TableViewModel.ConstrVm.ProductLb = _allProducts;

            TableViewModel.ConstrVm.DetailDeveloperTb = _detailDeveloperTb;
        }

        private void DesignerSelected(object sender, RoutedEventArgs e)
        {
            TableViewModel.ConstrVm.Init();
        }

        private void DetailSelected(object sender, RoutedEventArgs e)
        {
            ;
        }

        private void ProductionSelected(object sender, RoutedEventArgs e)
        {
            ;
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

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            MainContext.Materials.Load();
            var list = MainContext.Materials.Local.ToObservableCollection();

            _addMaterialsListBox.ItemsSource = list.OrderBy(x => x.Type);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
