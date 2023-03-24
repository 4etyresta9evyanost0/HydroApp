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
using Xceed.Wpf.Toolkit;

using Microsoft.EntityFrameworkCore;

namespace HydroApp
{
    /// <summary>
    /// Логика взаимодействия для AddNewConstruction.xaml
    /// </summary>
    public partial class AddNewConstruction : Window
    {
        public AddNewConstruction(HydropressDbContext context, IEnumerable<CommissionDetail> existingComDetails)
        {
            context.Constructions.Load();
            context.Details.Load();
            context.Productions.Load();
            InitializeComponent();


            Construction[] array = new Construction[context.Constructions.Local.Count];
            context.Constructions.Local.ToList().CopyTo(array);
            var list = new List<Construction>(array);
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (existingComDetails.Any(x => item.Id == x.IdConstruction))
                {
                    list.Remove(item);
                    i--;
                }
            }
            _addConstructionListBox.ItemsSource = list.OrderBy(x => x.Production != null);
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public List<(bool, Construction, int)>? Result
        {
            get
            {
                var result = new List<(bool, Construction, int)>();
                var list = (IOrderedEnumerable<Construction>)_addConstructionListBox.ItemsSource;
                for (int i = 0; i < list.Count(); i++)
                {
                    var datatemplate = _addConstructionListBox.ItemContainerGenerator.ContainerFromIndex(i);
                    var cp = (ContentPresenter)datatemplate;
                    var border = VisualTreeHelper.GetChild(cp, 0);
                    var stackpanel = VisualTreeHelper.GetChild(border, 0);

                    var cb = (CheckBox)VisualTreeHelper.GetChild(stackpanel, 0);
                    var upDown = (IntegerUpDown)VisualTreeHelper.GetChild(stackpanel, 2);
                    var constr = list.ElementAt(i);

                    result.Add(new(cb.IsChecked ?? false, constr, upDown.Value ?? 0));
                }
                return result;
            }
        }
    }
}
