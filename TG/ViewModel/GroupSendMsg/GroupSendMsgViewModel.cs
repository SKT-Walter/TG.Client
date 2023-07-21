using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.TG;

namespace TG.Client.ViewModel.GroupSendMsg
{
    public class GroupSendMsgViewModel : BaseViewModel
    {
        private System.Timers.Timer timer = null;

        private FrameworkElement ownUI = null;

        

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

        private string startInterval;
        public string StartInterval
        {
            get { return startInterval; }
            set
            {
                startInterval = value;

                this.OnPropertyChanged();
            }
        }

        private string endInterval;
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

        #region 按钮事件


        private void Init(FrameworkElement ownUI)
        {
            this.ownUI = ownUI;
        }

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

        public void SendGroupMsg()
        {
            UserHandler.Instance.PublishMsg("start send msg...");
            double interval = 0;
            if (timer != null)
            {
                timer.Stop();
                if (double.TryParse(StartInterval, out interval))
                {
                    // 设置定时器间隔，例如：1000表示每隔1秒触发一次
                    timer.Interval = interval * 1000 * 60;
                    
                    // 设置定时器自动重启
                    timer.AutoReset = true;
                    timer.Start();
                }
            }
            else
            {
                if (double.TryParse(StartInterval, out interval))
                {
                    // 设置定时器间隔，例如：1000表示每隔1秒触发一次
                    timer = new System.Timers.Timer(interval * 1000 * 60);
                    timer.Elapsed += Timer_Elapsed;
                    // 设置定时器自动重启
                    timer.AutoReset = true;
                    timer.Start();
                }
            }

            SendMsgPo sendMsgPo = new SendMsgPo();
            sendMsgPo.FilePath = FilePathChecked ? FilePath : string.Empty;
            sendMsgPo.SendMsg = sendMsg;

            BatchSendMsgHandler.Instance.SendBatchMsg(SendBatchUser, sendMsgPo);
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SendMsgPo sendMsgPo = new SendMsgPo();
            sendMsgPo.FilePath = FilePathChecked ? FilePath : string.Empty;
            sendMsgPo.SendMsg = sendMsg;

            BatchSendMsgHandler.Instance.SendBatchMsg(SendBatchUser, sendMsgPo);
        }

        #endregion

    }
}