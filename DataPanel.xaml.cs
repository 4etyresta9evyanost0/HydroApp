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

        public static event EventHandler? SelectedChanged;

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

        private void IntegerUpDown_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var ch = e.Text[0];
            if (!int.TryParse(ch + "", out _))
            {
                e.Handled = true;
            }
        }

        private void SelectFirst_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = 1;
        }
        private void SelectBack_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = SelectedValue == 1 ? SelectedValue : SelectedValue - 1;
        }

        private void SelectNext_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = SelectedValue == MaxValue ? SelectedValue : SelectedValue + 1 ;
        }

        private void SelectLast_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = MaxValue;
        }



        public RelayCommand? AddCommand
        {
            get { return (RelayCommand?)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(RelayCommand), typeof(DataPanel), new PropertyMetadata(null));

        public RelayCommand? RemoveCommand
        {
            get { return (RelayCommand?)GetValue(RemoveCommandProperty); }
            set { SetValue(RemoveCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveCommandProperty =
            DependencyProperty.Register("RemoveCommand", typeof(RelayCommand), typeof(DataPanel), new PropertyMetadata(null));

        public RelayCommand? SaveCommand
        {
            get { return (RelayCommand?)GetValue(SaveCommandProperty); }
            set { SetValue(SaveCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SaveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SaveCommandProperty =
            DependencyProperty.Register("SaveCommand", typeof(RelayCommand), typeof(DataPanel), new PropertyMetadata(null));



        public RelayCommand? UpdateCommand
        {
            get { return (RelayCommand?)GetValue(UpdateCommandProperty); }
            set { SetValue(UpdateCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UpdateCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UpdateCommandProperty =
            DependencyProperty.Register("UpdateCommand", typeof(RelayCommand), typeof(DataPanel), new PropertyMetadata(null));



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

    public class ZeroEnumeratorToOneEnumerator : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // zero to one
            if (value is int num)
            {
                return ++num; 
            }
            return null;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // one to zero
            if (value is int num)
            {
                return --num;
            }
            return null;
        }
    }
}
