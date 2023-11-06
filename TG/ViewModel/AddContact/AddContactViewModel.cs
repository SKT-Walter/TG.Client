using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TG.Client.BatchTG;
using TG.Client.Model;

namespace TG.Client.ViewModel.AddContact
{
    public class AddContactViewModel : BaseViewModel
    {
        private FrameworkElement ownUI = null;

        #region 界面属性
        
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


        #endregion

        #region 按钮事件

        public void OnClickSendBtn()
        {
            AddContactHandler addContactHandler = new AddContactHandler();

            addContactHandler.SendBatchMsg(sendBatchUser);
        }

        #endregion

        public void Init(FrameworkElement ownUI)
        {
            this.ownUI = ownUI;

        }

    }
}
