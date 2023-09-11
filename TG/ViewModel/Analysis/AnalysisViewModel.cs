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

        private int processPercent = 0;
        public int ProcessPercent
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
                int total = UserHandler.Instance.QuoteUserCount();

                int pageSize = 1000;
                int pageIndex = 0;
                while (true)
                {
                    List<TdUserPo> userList = UserHandler.Instance.QuoteUserByPage(pageSize, pageSize * pageIndex);

                    foreach (TdUserPo userPo in userList)
                    {
                        foreach (TdApi.Chat chat in AnalysisChatHandler.Instance.ChatList)
                        {
                            if (userPo.Flag.Contains(chat.Title))
                            {
                                string userJsonStr = JsonConvert.SerializeObject(userPo);
                                TdUserEx userEx = JsonConvert.DeserializeObject<TdUserEx>(userJsonStr);
                                userEx.GroupName = chat.Title;

                                UserHandler.Instance.SaveDexUser(userEx);
                            }
                        }
                    }
                    UserHandler.Instance.PublishMsg("StartCompare into next page...");
                    SetProcessPercent(1);
                }
            });
        }

        private void SetProcessPercent(int percent)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                ProcessPercent += percent;
            }));
        }

    }
}
