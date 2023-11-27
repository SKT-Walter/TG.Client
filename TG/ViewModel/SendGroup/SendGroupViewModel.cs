using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TG.Client.BatchTG;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;

namespace TG.Client.ViewModel.SendGroupViewModel
{
    public class SendGroupViewModel : BaseViewModel
    {

        private FrameworkElement ownUI = null;
        private ObservableCollection<TdGroupInfo> groupInfoList = new ObservableCollection<TdGroupInfo>();

        public ObservableCollection<TdGroupInfo> GroupInfoList
        {
            get { return groupInfoList; }
        }

        private System.Timers.Timer timer = null;

        #region 界面属性

        private bool filePathChecked;
        public bool FilePathChecked
        {
            get { return filePathChecked; }
            set
            {
                filePathChecked = value;

                this.OnPropertyChanged();
            }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;

                this.OnPropertyChanged();
            }
        }

        private string sendMsg;
        public string SendMsg
        {
            get { return sendMsg; }
            set
            {
                sendMsg = value;

                this.OnPropertyChanged();
            }
        }


        private string sendBatchUser;
        public string SendBatchUser
        {
            get { return sendBatchUser; }
            set
            {
                sendBatchUser = value;

                this.OnPropertyChanged();
            }
        }

        private string startInterval = "20";
        public string StartInterval
        {
            get { return startInterval; }
            set
            {
                startInterval = value;

                this.OnPropertyChanged();
            }
        }

        private string endInterval = "25";
        public string EndInterval
        {
            get { return endInterval; }
            set
            {
                endInterval = value;

                this.OnPropertyChanged();
            }
        }

        #endregion


        public void OnClickDownStartBtn()
        {
            int num = 0;
            if (int.TryParse(StartInterval, out num))
            {
                num -= 1;

                StartInterval = num >= 0 ? num.ToString() : "0";

            }
        }

        public void OnClickAddStartBtn()
        {
            int num = 0;
            if (int.TryParse(StartInterval, out num))
            {
                num += 1;

                StartInterval = num >= 0 ? num.ToString() : "0";

            }
            else
            {
                StartInterval = "1";
            }
        }


        public void OnClickDownEndBtn()
        {
            int num = 0;
            if (int.TryParse(EndInterval, out num))
            {
                num -= 1;

                EndInterval = num >= 0 ? num.ToString() : "0";

            }
        }

        public void OnClickAddEndBtn()
        {
            int num = 0;
            if (int.TryParse(EndInterval, out num))
            {
                num += 1;

                EndInterval = num >= 0 ? num.ToString() : "0";

            }
            else
            {
                EndInterval = "1";
            }
        }

        public void Init(FrameworkElement ownui)
        {
            this.ownUI = ownui;

            
        }


        public void SendGroupMsg()
        {
            UserHandler.Instance.PublishMsg("start send msg...");
            double interval = 0;
            

            SendMsgPo sendMsgPo = new SendMsgPo();
            sendMsgPo.FilePath = FilePathChecked ? FilePath : string.Empty;
            sendMsgPo.SendMsg = sendMsg;

            int start = 20;
            int end = 25;
            int.TryParse(StartInterval, out start);
            int.TryParse(EndInterval, out end);
            //单个发送
            //BatchSendMsgHandler.Instance.SendBatchMsg(SendBatchUser, sendMsgPo, start, end);

            //批量发送
            Dictionary<string, TGClient> dic = TGClientManager.Instance.GetAllClient();
            foreach (KeyValuePair<string, TGClient> kv in dic)
            {
                MoreClientBatchSendMsgHandler moreClientBatchSendMsgHandler = new MoreClientBatchSendMsgHandler(kv.Key, SendMsgType.None, sendMsgPo);

                moreClientBatchSendMsgHandler.SendBatchMsg(SendBatchUser, sendMsgPo, start, end);

                Thread.Sleep(1000);

            }

        }
        

    }

}
}
