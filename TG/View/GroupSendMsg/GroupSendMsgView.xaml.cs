﻿using Microsoft.Win32;
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
using TG.Client.Cache;
using TG.Client.ViewModel.GroupSendMsg;

namespace TG.Client.View.GroupSendMsg
{
    /// <summary>
    /// GroupSendMsgView.xaml 的交互逻辑
    /// </summary>
    public partial class GroupSendMsgView : UserControl
    {
        private GroupSendMsgViewModel groupSendMsgViewModel = null;

        public GroupSendMsgView()
        {
            InitializeComponent();

            groupSendMsgViewModel = new GroupSendMsgViewModel();

            this.DataContext = groupSendMsgViewModel;

            this.Loaded += GroupSendMsgView_Loaded;
        }

        private void GroupSendMsgView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= GroupSendMsgView_Loaded;

            FileMsgCache.Instance.Init();
        }

        private void BtnDownStart_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickDownStartBtn();
        }

        private void BtnAddStart_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickAddStartBtn();
        }

        private void BtnDownEnd_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickDownEndBtn();
        }

        private void BtnAddEnd_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickAddEndBtn();
        }

        private void BtnSendGroup_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.SendGroupMsg();
        }

        private void cbImageView_Checked(object sender, RoutedEventArgs e)
        {
            if (cbImageView.IsChecked.HasValue && cbImageView.IsChecked.Value)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //限制选择类型为图片
                openFileDialog.Filter = "图片文件(*.jpg,*.png)|*.jpg;*.png";
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    // 你可以在这里进行你需要的操作，例如显示图片，或者是读取图片的内容等等

                    groupSendMsgViewModel.FilePath = filePath;
                }
            }
            else
            {
                groupSendMsgViewModel.FilePath = string.Empty;
            }
        }

        private void cbImageView_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
