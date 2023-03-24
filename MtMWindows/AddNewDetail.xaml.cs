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
    /// Логика взаимодействия для AddNewDetail.xaml
    /// </summary>
    public partial class AddNewDetail : Window
    {
        public AddNewDetail(HydropressDbContext context, IEnumerable<DetailsForProduction> existingDetails)
        {
            InitializeComponent();
            context.Constructions.Load();
            context.Details.Load();

            Detail[] array = new Detail[context.Details.Local.Count];
            context.Details.Local.ToList().CopyTo(array);
            var list = new List<Detail>(array);
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (existingDetails.Any(x => item.Id == x.IdDetail))
                {
                    list.Remove(item);
                    i--;
                }
            }
            _addDetailsListBox.ItemsSource = list.OrderBy(x => x.Purpose);
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public List<(bool, Detail, int)>? Result
        {
            get
            {
                var result = new List<(bool, Detail, int)>();
                var list = (IOrderedEnumerable<Detail>)_addDetailsListBox.ItemsSource;
                for (int i = 0; i < list.Count(); i++)
                {
                    var datatemplate = _addDetailsListBox.ItemContainerGenerator.ContainerFromIndex(i);
                    var cp = (ContentPresenter)datatemplate;
                    var border = VisualTreeHelper.GetChild(cp, 0);
                    var stackpanel = VisualTreeHelper.GetChild(border, 0);

                    var cb = (CheckBox)VisualTreeHelper.GetChild(stackpanel, 0);
                    var upDown = (IntegerUpDown)VisualTreeHelper.GetChild(stackpanel, 2);
                    var detail = list.ElementAt(i);

                    result.Add(new(cb.IsChecked ?? false, detail, upDown.Value ?? 0));
                }
                return result;
            }
        }
    }
}
