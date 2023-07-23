using Newtonsoft.Json;
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
using TG.Client.Handler;
using TG.Client.Utils.SqlLite;
using TG.Client.ViewModel.GroupSendMsg;
using TG.ViewModel.APICollect;

namespace TG
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UserHandler.Instance.OnChange += MainWindow_OnChange;
        }


        private void MainWindow_OnChange(object obj)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                tbLog.Text += string.Format("{0}：{1}\n", DateTime.Now.ToString("HH:mm:ss"), JsonConvert.SerializeObject(obj));
                if (tbLog.Text.Length > 50000)
                {
                    tbLog.Text = string.Empty;
                }
            }));
        }
    }
}
