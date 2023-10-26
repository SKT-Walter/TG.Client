using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TG.Client.BatchTG;
using TG.Client.Cache;
using TG.Client.Model;
using TG.Client.Utils;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

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


            //Td.Client.Execute(new TdApi.SetLogVerbosityLevel(0));
            //if (Td.Client.Execute(new TdApi.SetLogStream(new TdApi.LogStreamFile("tdlib.log", 1 << 27, false))) is TdApi.Error)
            //{
            //    throw new System.IO.IOException("Write access to the current directory is required");
            //}
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Td.Client.Run();
            }).Start();
        }

        private void ReadAccount()
        {
            string[] lines = FileHelper.ReadFile("./data/TG_ID.txt");

            foreach (string line in lines)
            {
                string[] arr = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                LoginViewModel tem = new LoginViewModel()
                {
                    Account = arr[0],//"Leland1",
                    Status = "1",
                    StatusBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 42, 25)),
                    Phone = arr[2],//"18201920475",
                    PhonePrefix = arr[1],//"+86",
                    APIID = arr[3],//"26272692",
                    APIHASH = arr[4]//"241a6f347f3b88e5bb7ca38f148e2bdb"
                };

                //_accountData.Add(tem);
            }

            LoginViewModel one = new LoginViewModel()
            {
                Account = "Leland1",
                Status = "1",
                StatusBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 42, 25)),
                Phone = "18201920475",
                PhonePrefix = "+86",
                APIID = "26272692",
                APIHASH = "241a6f347f3b88e5bb7ca38f148e2bdb"
            };

            LoginViewModel two = new LoginViewModel()
            {
                Account = "Willow Amery",
                Status = "0",
                StatusBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 42, 25)),
                Phone = "02319457",
                PhonePrefix = "+888",
                APIID = "28406007",
                APIHASH = "cab10c8a4c0e9c99a180b3c7fc6c6aae"
            };


            //_accountData.Add(one);
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
                    client.CreateTdClient(selectRow);
                    TGClientManager.Instance.AddOrUpdate(client, selectRow.Account);
                }

                client.ProcessLogin();
            }
        }
    }
}
