using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
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
    /// Логика взаимодействия для DataPanel.xaml
    /// </summary>
    public partial class DataPanel : UserControl
    {

        public int SelectedValue
        {
            get { return (int)GetValue(SelectedValueProperty); }
            set
            {
                SetValue(SelectedValueProperty, value);
                SelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(int), typeof(DataPanel), new PropertyMetadata(0));



        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
                SelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(DataPanel), new PropertyMetadata(null));


        public static event EventHandler? SelectedChanged;


        //public ObservableCollection<object> Collection
        //{
        //    get { return (ObservableCollection<object>)GetValue(CollectionProperty); }
        //    set
        //    {
        //        SetValue(CollectionProperty, value);
        //        MaxValue = value.Count;
        //    }
        //}

        //// Using a DependencyProperty as the backing store for Collection.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CollectionProperty =
        //    DependencyProperty.Register("Collection", typeof(ObservableCollection<object>), typeof(DataPanel), new PropertyMetadata(null, OnCollectionChanged));

        private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var old = (ObservableCollection<object>)e.OldValue;
            if (old != null)
            {
                old.CollectionChanged -= collectionChanged;
            }
            if (e.NewValue == null)
                return;
            ((ObservableCollection<object>)e.NewValue).CollectionChanged += collectionChanged;
            // = ((ObservableCollection<object>)e.NewValue).Count;
            //CollectionChanged?.Invoke(this, e);
        }

        public static event NotifyCollectionChangedEventHandler? CollectionChanged;

        static void collectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
        {
            //throw new NotImplementedException();
            CollectionChanged?.Invoke(sender, e);
        }


        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        //Using a DependencyProperty as the backing store for Max.This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(DataPanel), new PropertyMetadata(0));

        public DataPanel()
        {
            InitializeComponent();
        }
    }

    public class StringToIntConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                if (int.TryParse(str, out int intVal))
                    return intVal;
                else
                    return null;
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int num)
            {
                return num.ToString();
            }
            return null;
        }
    }
}
