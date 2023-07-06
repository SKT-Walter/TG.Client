﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.Model.Login;
using TG.Client.TG;
using TG.Client.Utils;

namespace TG.ViewModel.APICollect
{
    public class APICollectViewModel : BaseViewModel, IMessage
    {
        private FrameworkElement ownUI = null;
        private AsyncThreadQueue<TdUserPo> processUserThreadQueue;
        private ObservableCollection<TdUserPo> userList = new ObservableCollection<TdUserPo>();

        public event Action<object> OnChange;

        public ObservableCollection<TdUserPo> UserList
        {
            get { return userList; }
        }

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


        private bool inner24Hour = false;

        public bool Inner24Hour
        {
            get { return inner24Hour; }
            set
            {
                inner24Hour = value;

                this.OnPropertyChanged();
            }
        }

        private bool inner7Day = false;

        public bool Inner7Day
        {
            get { return inner7Day; }
            set
            {
                inner7Day = value;

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
            userList.Clear();

            int num = 0;
            int.TryParse(CollectNum, out num);
            TdClientHandler.Instance.CollectUser(InviteLink, Inner24Hour ? TimeFilterType.OneDay : Inner7Day ? TimeFilterType.SevenDay : TimeFilterType.None, num);
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


            processUserThreadQueue = new AsyncThreadQueue<TdUserPo>(OnUserChange);

            TdClientHandler.Instance.OnUserChange += Instance_OnUserChange;
            TdClientHandler.Instance.CreateTdClient(this);
        }

        private void OnUserChange(TdUserPo userPo)
        {
            UserHandler.Instance.SaveUser(userPo);

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                UserList.Add(userPo);
            }));

           
        }

        private void Instance_OnUserChange(Telegram.Td.Api.User user)
        {
            TdUserPo userPo = new TdUserPo();
            userPo.UserId = user.Id;
            userPo.FirstName = user.FirstName;
            userPo.LastName = user.LastName;
            userPo.PhoneNumber = user.PhoneNumber;

            processUserThreadQueue.Enqueue(userPo);
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


        public void RequestExportOrders(string fileName)
        {
            StreamWriter sw = null;
            try
            {
                sw = ExcelHelper.Instance.createStream(fileName);
                if (sw != null)
                {
                    Console.WriteLine("fileName:" + fileName);
                    //N:已申请,0:处理中，1,2:已确认(其中1为部分确认,2完全确认)，4:已撤销，8:失败
                    //E(Broker B确认),F(Broker B撤销),G（Broker A确认），H（Broker A撤销）
                    //request.orderStatus = "N,0,1";
                    
                    foreach (TdUserPo tdUserPo in UserList)
                    {

                    }

                }
            }
            catch (Exception e)
            {

            }
        }
    }
}
