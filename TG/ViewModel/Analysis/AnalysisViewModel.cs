using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.TG;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.ViewModel.Analysis
{
    public class AnalysisViewModel : BaseViewModel
    {
        

        private FrameworkElement ownUI = null;
        private ObservableCollection<TdUserEx> userList = new ObservableCollection<TdUserEx>();

        public ObservableCollection<TdUserEx> UserList
        {
            get { return userList; }
        }

        private double processPercent = 0;
        public double ProcessPercent
        {
            get { return processPercent; }
            set
            {
                processPercent = value;

                this.OnPropertyChanged();
            }
        }


        public void Init(FrameworkElement ownui)
        {
            this.ownUI = ownui;


            CommonHandler.Instance.OnStartAnalysis += Instance_OnStartAnalysis;
        }

        private void Instance_OnStartAnalysis()
        {
            SetProcessPercent(10);
            this.StartCompare();

        }

        public void StartAnalysis()
        {
            UserHandler.Instance.PublishMsg("start analysis...");
            ProcessPercent = 5;
            Task.Run(() =>
            {
                AnalysisChatHandler.Instance.GetChats();
                
            });
        }

        public void StartCompare()
        {
            Task.Run(() =>
            {
                Int64 total = UserHandler.Instance.QuoteUserCount();
                decimal totalD = 0;
                decimal.TryParse(total.ToString(), out totalD);
                int pageSize = 1000;
                int pageIndex = 0;
                decimal temD = totalD / pageSize;
                int temInt = (int)temD;
                int totalPage = temD > temInt ? (temInt + 1) : temInt;
                double percent = 90.0;
                double step = percent / totalPage;
                while (true)
                {
                    List<TdUserPo> userList = UserHandler.Instance.QuoteUserByPage(pageSize, pageSize * pageIndex);
                    pageIndex++;
                    foreach (TdUserPo userPo in userList)
                    {
                        foreach (string groupName in AnalysisChatHandler.Instance.GetGroupNameHs.Keys)
                        {
                            if (userPo.Flag.Contains(groupName))
                            {
                                string userJsonStr = JsonConvert.SerializeObject(userPo);
                                TdUserEx userEx = JsonConvert.DeserializeObject<TdUserEx>(userJsonStr);
                                userEx.GroupName = groupName;

                                UserHandler.Instance.SaveDexUser(userEx);

                                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                                {
                                    UserList.Add(userEx);
                                }));
                            }
                        }
                    }

                    UserHandler.Instance.PublishMsg("StartCompare into next page:" + pageIndex + ", total page:" + totalPage);
                    
                    SetProcessPercent(step);

                    if (userList == null || userList.Count < pageSize)
                    {
                        break;
                    }
                }

                UserHandler.Instance.PublishMsg("分析数据完成...");
            });
        }

        private void SetProcessPercent(double percent)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                ProcessPercent += percent;
            }));
        }

    }
}
