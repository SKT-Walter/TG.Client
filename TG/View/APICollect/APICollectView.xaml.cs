﻿using Microsoft.Win32;
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
using TG.Client.Model;
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

        private void MenuItem_CopySelectedRows_Click(object sender, RoutedEventArgs e)
        {
            var selectedRows = dgData.SelectedItems;
            StringBuilder copyData = new StringBuilder();

            // 遍历每一行
            foreach (TdUserPo row in selectedRows)
            {
                copyData.Append(row.UserId + "," + row.Name + "," + row.PhoneNumber);
                
                copyData.AppendLine();
            }

            // 将数据复制到剪贴板
            Clipboard.SetText(copyData.ToString());
        }

        private void DataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // 检查是否按下Ctrl+C
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.C)
                {
                    MenuItem_CopySelectedRows_Click(sender, null);
                    e.Handled = true;
                }
            }
        }

    }
}
