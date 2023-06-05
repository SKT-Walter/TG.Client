using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TG.Client.Model;
using TG.Client.Model.Login;
using TG.Client.TG;

namespace TG.ViewModel.APICollect
{
    public class APICollectViewModel : BaseViewModel, IMessage
    {
        private FrameworkElement ownUI = null;

        #region 属性

        private string phonePrefix;

        public string PhonePrefix
        {
            get { return phonePrefix; }
            set
            {
                phonePrefix = value;

                this.OnPropertyChanged();
            }
        }

        private string phone;

        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;

                this.OnPropertyChanged();
            }
        }

        private string verifyCode;

        public string VerifyCode
        {
            get { return verifyCode; }
            set
            {
                verifyCode = value;

                this.OnPropertyChanged();
            }
        }
        
        private string inviteLink;

        public string InviteLink
        {
            get { return inviteLink; }
            set
            {
                inviteLink = value;

                this.OnPropertyChanged();
            }
        }

        private string collectNum;

        public string CollectNum
        {
            get { return collectNum; }
            set
            {
                collectNum = value;

                this.OnPropertyChanged();
            }
        }

        private string verifyMsg = "需要先验证才能使用API采集";

        public string VerifyMsg
        {
            get { return verifyMsg; }
            set
            {
                verifyMsg = value;

                this.OnPropertyChanged();
            }
        }

        private Brush verifyForeground;

        public Brush VerifyForeground
        {
            get { return verifyForeground; }
            set
            {
                verifyForeground = value;

                this.OnPropertyChanged();
            }
        }

        private int collectProcess = 0;

        public int CollectProcess
        {
            get { return collectProcess; }
            set
            {
                collectProcess = value;

                this.OnPropertyChanged();
            }
        }

        #endregion

        private LoginPo GetLoginPo()
        {
            LoginPo loginPo = new LoginPo();
            loginPo.AuthenticationCode = VerifyCode;
            loginPo.Phone = PhonePrefix + " " + Phone;


            return loginPo;
        }

        #region 按钮事件

        public void OnClickSendBtn()
        {
            TdClientHandler.Instance.ProcessLogin(GetLoginPo());
        }

        public void OnClickVerifyCodeBtn()
        {
            TdClientHandler.Instance.ProcessLogin(GetLoginPo());
        }

        public void OnClickCollectBtn()
        {
            TdClientHandler.Instance.CollectUser(InviteLink);
        }

        public void OnClickDownBtn()
        {
            int num = 0;
            if (int.TryParse(CollectNum, out num))
            {
                num -= 1;

                CollectNum = num>=0?num.ToString():"0";
                
            }
        }

        public void OnClickAddBtn()
        {
            int num = 0;
            if (int.TryParse(CollectNum, out num))
            {
                num += 1;

                CollectNum = num >= 0 ? num.ToString() : "0";

            }
            else
            {
                CollectNum = "1";
            }
        }

        public void OnClickTestBtn()
        {
            TdClientHandler.Instance.GetCommand(GetLoginPo(), "me");
        }
        
        #endregion


        public void Init(FrameworkElement ownui)
        {
            this.ownUI = ownui;

            VerifyForeground = ownUI.FindResource("Black-1") as Brush;


            TdClientHandler.Instance.CreateTdClient(this);
        }

        public void OnMessage(object obj)
        {
            BaseReplyPo baseReply = obj as BaseReplyPo;
            if (baseReply != null)
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if (baseReply.Code == "1100")
                    {
                        this.VerifyForeground = this.ownUI.FindResource("Orange-1") as Brush;
                    }
                    else if (baseReply.IsSuccess())
                    {
                        this.VerifyForeground = this.ownUI.FindResource("Green-1") as Brush;
                    }
                    else
                    {
                        this.VerifyForeground = this.ownUI.FindResource("Orange-2") as Brush;
                    }
                    this.VerifyMsg = baseReply.Msg;

                }));
            }
        }
    }
}
