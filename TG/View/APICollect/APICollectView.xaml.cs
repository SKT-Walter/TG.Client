using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using TG.Client.Utils;
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private string fileName = string.Empty;
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "(文本文件*.csv)|*.csv";
            //saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_采集用户";
            //saveFileDialog.AddExtension = true;
            //saveFileDialog.RestoreDirectory = true;
            //if ((bool)(saveFileDialog.ShowDialog()))
            //{
            //    fileName = saveFileDialog.FileName;
            //    //ThreadPool.QueueUserWorkItem(new WaitCallback(RequestExportOrders));


            //    //RequestExportOrders(null);
            //}
        }


        
    }
}
