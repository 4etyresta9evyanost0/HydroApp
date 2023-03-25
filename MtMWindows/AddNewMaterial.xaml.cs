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

using Microsoft.EntityFrameworkCore;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.Toolkit;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewMaterial.xaml
    /// </summary>
    public partial class AddNewMaterial : Window
    {
        public AddNewMaterial(HydropressDbContext context, IEnumerable<MaterialsForDetail> existingMaterials)
        {
            InitializeComponent();
            context.Materials.Load();
            Material[] array = new Material[context.Materials.Local.Count];
            context.Materials.Local.ToList().CopyTo(array);
            var list = new List<Material>(array);
            for (int i = 0; i < list.Count; i++ )
            {
                var item = list[i];
                if (existingMaterials.Any(x=> item.Id == x.IdMaterial))
                {
                    list.Remove(item);
                    i--;
                }
            }
            _addMaterialsListBox.ItemsSource = list.OrderBy(x => x.Type);
        }

        public AddNewMaterial(HydropressDbContext context, IEnumerable<SupplyDetail> existingMaterials)
        {
            InitializeComponent();
            context.Materials.Load();
            Material[] array = new Material[context.Materials.Local.Count];
            context.Materials.Local.ToList().CopyTo(array);
            var list = new List<Material>(array);
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (existingMaterials.Any(x => item.Id == x.IdMaterial))
                {
                    list.Remove(item);
                    i--;
                }
            }
            _addMaterialsListBox.ItemsSource = list.OrderBy(x => x.Type);
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public List<(bool, Material, double)>? Result 
        {
            get
            {
                var result = new List<(bool, Material, double)>();
                var list = (IOrderedEnumerable<Material>)_addMaterialsListBox.ItemsSource;
                for (int i = 0; i < list.Count(); i++)
                {
                    var datatemplate = _addMaterialsListBox.ItemContainerGenerator.ContainerFromIndex(i);
                    var cp = (ContentPresenter)datatemplate;
                    var border = VisualTreeHelper.GetChild(cp, 0);
                    var stackpanel = VisualTreeHelper.GetChild(border, 0);

                    var cb = (CheckBox)VisualTreeHelper.GetChild(stackpanel, 0);
                    var upDown = (DoubleUpDown)VisualTreeHelper.GetChild(stackpanel, 2);
                    var material = list.ElementAt(i);

                    result.Add(new(cb.IsChecked ?? false, material, upDown.Value ?? 0));
                }
                return result;
            } 
        }
    }
}
