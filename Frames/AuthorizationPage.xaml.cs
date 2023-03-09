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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        AuthorizationViewModel AuthVm { get => (AuthorizationViewModel)Application.Current.Resources["AuthVm"]; }
        MainViewModel Mvm { get => (MainViewModel)Application.Current.Resources["Mvm"]; }
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            await Mvm.Authorize(loginTb.Text, passwordTb.Password);
            passwordTb.Clear();
        }

        private void loginTb_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                passwordTb.Focus();
        }

        private void passwordTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Button_Click(sender, e);
        }
    }
}
