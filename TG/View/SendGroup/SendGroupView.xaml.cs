using Microsoft.Win32;
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
using TG.Client.ViewModel.SendGroupViewModel;

namespace TG.Client.View.SendGroup
{
    /// <summary>
    /// SendGroupView.xaml 的交互逻辑
    /// </summary>
    public partial class SendGroupView : UserControl
    {
        private SendGroupViewModel sendGroupViewModel = null;


        public SendGroupView()
        {
            InitializeComponent();

            sendGroupViewModel = new SendGroupViewModel();

            this.DataContext = sendGroupViewModel;
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

                    sendGroupViewModel.FilePath = filePath;
                }
            }
            else
            {
                sendGroupViewModel.FilePath = string.Empty;
            }
        }

        private void BtnDownStart_Click(object sender, RoutedEventArgs e)
        {
            sendGroupViewModel.OnClickDownStartBtn();
        }

        private void BtnAddStart_Click(object sender, RoutedEventArgs e)
        {
            sendGroupViewModel.OnClickAddStartBtn();
        }

        private void BtnDownEnd_Click(object sender, RoutedEventArgs e)
        {
            sendGroupViewModel.OnClickDownEndBtn();
        }

        private void BtnAddEnd_Click(object sender, RoutedEventArgs e)
        {
            sendGroupViewModel.OnClickAddEndBtn();
        }

        private void BtnSendGroup_Click(object sender, RoutedEventArgs e)
        {
            sendGroupViewModel.SendGroupMsg();
        }

        private void BtnGetGroup_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
