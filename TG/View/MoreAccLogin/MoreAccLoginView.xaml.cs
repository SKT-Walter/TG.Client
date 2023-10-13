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
    /// MoreAccLoginView.xaml 的交互逻辑
    /// </summary>
    public partial class MoreAccLoginView : UserControl
    {
        private MoreAccLoginViewModel moreAccLoginViewModel = null;

        public MoreAccLoginView()
        {
            InitializeComponent();

            moreAccLoginViewModel = new MoreAccLoginViewModel();

            this.DataContext = moreAccLoginViewModel;

            moreAccLoginViewModel.Init(this);
        }

        private void DataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            moreAccLoginViewModel.ProcessLogin();
        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            moreAccLoginViewModel.ProcessLogin();
        }
    }
}
