﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TG.Client.BatchTG;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;
using TG.Client.Model.Login;
using TG.Client.TG;
using TG.Client.Utils;

namespace TG.ViewModel.APICollect
{
    public class APICollectViewModel : BaseViewModel, IMessage, ICollectMessage
    {
        private FrameworkElement ownUI = null;
        private AsyncThreadQueue<TdUserPo> processUserThreadQueue;
        private ObservableCollection<TdUserPo> userList = new ObservableCollection<TdUserPo>();
        
        public ObservableCollection<TdUserPo> UserList
        {
            get { return userList; }
        }

        #region 属性

        private int collectedGroupUserNum;
        public int CollectedGroupUserNum
        {
            get { return collectedGroupUserNum; }
            set
            {
                collectedGroupUserNum = value;

                this.OnPropertyChanged();
            }
        }


        private int groupTotal;
        public int GroupTotal
        {
            get { return groupTotal; }
            set
            {
                groupTotal = value;

                this.OnPropertyChanged();
            }
        }

        private int collectedUserNum;
        public int CollectedUserNum
        {
            get { return collectedUserNum; }
            set
            {
                collectedUserNum = value;

                this.OnPropertyChanged();
            }
        }

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
        

        private int noBackTotal;
        public int NoBackTotal
        {
            get { return noBackTotal; }
            set
            {
                noBackTotal = value;

                this.OnPropertyChanged();
            }
        }


        private int filterBotTotal;
        public int FilterBotTotal
        {
            get { return filterBotTotal; }
            set
            {
                filterBotTotal = value;

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
            UserHandler.Instance.PublishMsg("start collect...");

            userList.Clear();
            GroupTotal = CollectedGroupUserNum = CollectedUserNum = 0;
            int num = 0;
            int.TryParse(CollectNum, out num);
            //单个采集
            //TdClientHandler.Instance.CollectUser(InviteLink, Inner24Hour ? TimeFilterType.OneDay : Inner7Day ? TimeFilterType.SevenDay : TimeFilterType.None, num);



            BatchTaskManager.Instance.Clear();
            //批量采集
            BatchCollectUser batchCollectUser = new BatchCollectUser();
            batchCollectUser.SetCollectMessage(this);
            string[] linkArr = InviteLink.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string link in linkArr)
            {
                batchCollectUser.CollectUser(link, Inner24Hour ? TimeFilterType.OneDay : Inner7Day ? TimeFilterType.SevenDay : TimeFilterType.None, num);
            }

            batchCollectUser.StartCollect();
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

            CommonHandler.Instance.OnGetMembers += Instance_OnGetMembers;
            CommonHandler.Instance.OnUserChange += Instance_OnUserChange1;

            VerifyForeground = ownUI.FindResource("Black-1") as Brush;

            UserHandler.Instance.Init();

            processUserThreadQueue = new AsyncThreadQueue<TdUserPo>(OnUserChange);

            TdClientHandler.Instance.OnUserChange += Instance_OnUserChange;
            //TdClientHandler.Instance.CreateTdClient(this);

            TdClientHandler.Instance.OnBotUserChange += Instance_OnBotUserChange;
            TdClientHandler.Instance.OnFailUserChange += Instance_OnFailUserChange;
        }

        private void Instance_OnUserChange1(Telegram.Td.Api.User user)
        {
            this.Instance_OnUserChange(user);
        }

        private void Instance_OnFailUserChange(int obj)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                NoBackTotal = obj;

            }));
        }

        public void Instance_OnBotUserChange(Telegram.Td.Api.User obj)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                FilterBotTotal += 1;

            }));
        }

        public void Instance_OnGetMembers(int arg1, int arg2)
        {
            GroupTotal = arg1;
            CollectedGroupUserNum += arg2;
        }

        public void OnUserChange(TdUserPo userPo)
        {
            UserHandler.Instance.SaveUser(userPo);

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                CollectedUserNum += 1;
                UserList.Add(userPo);
                
            }));

           
        }

        public void Instance_OnUserChange(Telegram.Td.Api.User user)
        {
            TdUserPo userPo = new TdUserPo();
            userPo.UserId = user.Id;
            userPo.FirstName = user.FirstName;
            userPo.LastName = user.LastName;
            userPo.PhoneNumber = user.PhoneNumber;
            userPo.Flag = user.RestrictionReason;

            if (user.Usernames != null)
            {

                userPo.Username = userPo.EditableUsername = user.Usernames.EditableUsername;
                if (user.Usernames.ActiveUsernames != null)
                {
                    foreach (string str in user.Usernames.ActiveUsernames)
                    {
                        userPo.ActiveUsernames += str + ",";
                    }

                    if (userPo.ActiveUsernames != null && userPo.ActiveUsernames.Length > 0)
                    {
                        userPo.ActiveUsernames = userPo.ActiveUsernames.Substring(0, userPo.ActiveUsernames.Length - 1);
                    }
                }
                if (user.Usernames.DisabledUsernames != null)
                {
                    foreach (string str in user.Usernames.DisabledUsernames)
                    {
                        userPo.DisabledUsernames += str + ",";
                    }

                    if (userPo.DisabledUsernames != null && userPo.DisabledUsernames.Length > 0)
                    {
                        userPo.DisabledUsernames = userPo.DisabledUsernames.Substring(0, userPo.DisabledUsernames.Length - 1);
                    }
                }
            }
            

            processUserThreadQueue.Enqueue(userPo);
        }

        public void OnMessage(object obj)
        {
            UserHandler.Instance.PublishMsg(obj);
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
