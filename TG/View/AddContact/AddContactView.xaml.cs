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
using TG.Client.ViewModel.AddContact;

namespace TG.Client.View.AddContact
{
    /// <summary>
    /// AddContactView.xaml 的交互逻辑
    /// </summary>
    public partial class AddContactView : UserControl
    {
        private AddContactViewModel addContactViewModel = null;
        public AddContactView()
        {
            InitializeComponent();

            addContactViewModel = new AddContactViewModel();

            this.DataContext = addContactViewModel;

            addContactViewModel.Init(this);
        }

        private void BtnSendGroup_Click(object sender, RoutedEventArgs e)
        {
            addContactViewModel.OnClickSendBtn();
        }
    }
}
