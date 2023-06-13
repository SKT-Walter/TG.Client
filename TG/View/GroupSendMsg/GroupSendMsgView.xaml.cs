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
        }


        private void BtnDownStart_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickDownStartBtn();
        }

        private void BtnAddStart_Click(object sender, RoutedEventArgs e)
        {
            groupSendMsgViewModel.OnClickDownStartBtn();
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
    }
}
