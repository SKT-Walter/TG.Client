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
using TG.ViewModel.APICollect;

namespace TG.Client.View.APICollect
{
    /// <summary>
    /// APICollectView.xaml 的交互逻辑
    /// </summary>
    public partial class APICollectView : UserControl
    {
        private APICollectViewModel apiCollectViewModel = null;

        public APICollectView()
        {
            InitializeComponent();

            apiCollectViewModel = new APICollectViewModel();
            this.DataContext = apiCollectViewModel;

            apiCollectViewModel.Init(this);
        }

        private void BtnCollect_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickCollectBtn();
        }

        private void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickSendBtn();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnVerify_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickVerifyCodeBtn();
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickDownBtn();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickAddBtn();
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            apiCollectViewModel.OnClickTestBtn();
        }
    }
}
