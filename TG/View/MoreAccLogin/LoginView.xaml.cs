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
using TG.Client.ViewModel.MoreAccLogin;

namespace TG.Client.View.MoreAccLogin
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : UserControl
    {
        private LoginViewModel loginViewModel = new LoginViewModel();

        public LoginView()
        {
            InitializeComponent();

            this.DataContext = loginViewModel;
        }

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
