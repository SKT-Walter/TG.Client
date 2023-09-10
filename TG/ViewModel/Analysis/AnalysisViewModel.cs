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

        private void Instance_OnStartAnalysis(object obj)
        {
            if (obj is TdApi.Chats)
            {
                this.StartCompare(obj as TdApi.Chats);
            }
        }

        public void StartAnalysis()
        {
            UserHandler.Instance.PublishMsg("start analysis...");
            ProcessPercent = 5;
            Task.Run(() =>
            {
                AnalysisChatHandler.Instance.GetChat();
                
            });
        }

        public void StartCompare(TdApi.Chats chats)
        {
            Task.Run(() =>
            {
                int total = UserHandler.Instance.QuoteUserCount();

                int pageSize = 1000;
                int pageIndex = 0;
                while (true)
                {
                    List<TdUserPo> userList = UserHandler.Instance.QuoteUserByPage(pageSize, pageSize * pageIndex);
                }
            });
        }

    }
}
