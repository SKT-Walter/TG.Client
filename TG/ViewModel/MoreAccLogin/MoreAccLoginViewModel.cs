using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TG.Client.BatchTG;
using TG.Client.Cache;
using TG.Client.Model;

namespace TG.Client.ViewModel.MoreAccLogin
{
    public class MoreAccLoginViewModel : BaseViewModel
    {
        private FrameworkElement _myOwn = null;

        #region 数据源

        private ObservableCollection<LoginViewModel> _accountData = new ObservableCollection<LoginViewModel>();

        public ObservableCollection<LoginViewModel> AccountData
        {
            get { return _accountData; }
        }

        private LoginViewModel selectRow;
        public LoginViewModel SelectRow
        {
            get { return selectRow; }
            set { selectRow = value; }
        }

        #endregion

        public void Init(FrameworkElement ownUI)
        {
            _myOwn = ownUI;


            this.ReadAccount();
        }

        private void ReadAccount()
        {
            LoginViewModel one = new LoginViewModel()
            {
                Account = "Leland1",
                Status = "1",
                StatusBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 42, 25)),
                Phone = "1231231",
                PhonePrefix = "86",
                VerifyCode = "123"
            };

            LoginViewModel two = new LoginViewModel()
            {
                Account = "Leland2",
                Status = "0",
                StatusBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(39, 220, 28)),
                Phone = "2342342",
                PhonePrefix = "26",
                VerifyCode = "12323"
            };


            _accountData.Add(one);
            _accountData.Add(two);
        }

        public void ProcessLogin()
        {
            if (selectRow != null)
            {
                TGClient client = TGClientManager.Instance.GetClientByAcc(selectRow.Account);
                if (client == null)
                {
                    client = new TGClient();
                    TGClientManager.Instance.AddOrUpdate(client, selectRow.Account);
                }

                client.ProcessLogin(selectRow);
            }
        }
    }
}
