using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;

namespace TG.Client.ViewModel.GroupSendMsg
{
    public class GroupSendMsgViewModel : BaseViewModel
    {
        #region 界面属性

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

        }

        #endregion

    }
}