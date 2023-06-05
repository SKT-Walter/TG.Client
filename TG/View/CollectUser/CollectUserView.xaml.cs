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
using TG.ViewModel.CollectUser;

namespace TG.Client.View.CollectUser
{
    /// <summary>
    /// CollectUserView.xaml 的交互逻辑
    /// </summary>
    public partial class CollectUserView : UserControl
    {
        private CollectUserViewModel collectUserViewModel = null;

        public CollectUserView()
        {
            InitializeComponent();

            collectUserViewModel = new CollectUserViewModel();
            this.DataContext = collectUserViewModel;
        }

       

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCollect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
