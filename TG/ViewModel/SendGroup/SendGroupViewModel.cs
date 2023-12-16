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
using TG.Client.Model.Login;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.ViewModel.SendGroupViewModel
{
    public class SendGroupViewModel : BaseViewModel, Td.ClientResultHandler
    {
        private long txtMsgId = 0;
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

        public void GetGroup()
        {
            UserHandler.Instance.PublishMsg("start get group");

            Dictionary<string, TGClient> dic = TGClientManager.Instance.GetAllClient();
            foreach (KeyValuePair<string, TGClient> kv in dic)
            {
                TdApi.GetChats getChats = new TdApi.GetChats(new TdApi.ChatListMain(), 200);
                kv.Value.GetClient().Send(getChats, this);
            }
        }

        public void SendGroupMsg()
        {
            UserHandler.Instance.PublishMsg("start send msg...");
            

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

            TGClient tGClient = GetOneClient();
            Task.Run(() =>
            {
                foreach (TdGroupInfo tdGroupInfo in groupInfoList)
                {
                    this.SendMessage(tGClient, tdGroupInfo.GroupId, null);
                    this.SendImageMsg(tGClient, tdGroupInfo.GroupId, null);
                    int nextInterval = new Random().Next(start, end);
                    UserHandler.Instance.PublishMsg("下一次发送消息间隔：" + nextInterval);
                    Thread.Sleep(nextInterval * 1000);
                }
            });
        }

        private void SendMessage(TGClient tGClient, long chatId, SendMsgPo message)
        {
            // initialize reply markup just for testing
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };

            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });

            string sendMsg = FileMsgCache.Instance.GetMsg();
            UserHandler.Instance.PublishMsg("发送消息：" + sendMsg);
            if (!string.IsNullOrEmpty(sendMsg))
            {
                TdApi.InputMessageContent content = new TdApi.InputMessageText(new TdApi.FormattedText(sendMsg, null), false, true);
                //new TdApi.InputMessageText(new TdApi.FormattedText(message.SendMsg, null), false, true);

                tGClient.GetClient().Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, content), this);
                
            }

        }

        private void SendImageMsg(TGClient tGClient, long chatId, SendMsgPo message)
        {
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };

            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });
            // 发送图片消息
            string filePath = FileMsgCache.Instance.GetImage();
            UserHandler.Instance.PublishMsg("发送图片路径：" + filePath);
            //if (!string.IsNullOrEmpty(message.FilePath))
            {
                TdApi.InputMessageContent photoMessage = new TdApi.InputMessagePhoto
                {
                    Photo = new TdApi.InputFileLocal(filePath),// message.FilePath),//(@"C:\photos\my-photo.jpg"),  // 这里你需要使用实际图片路径
                    Caption = new TdApi.FormattedText { Text = string.Empty },//"图片说明" },  //图片下面会显示的文字

                };
                tGClient.GetClient().Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, photoMessage), this);
            }
        }

        private TGClient GetOneClient()
        {
            TGClient tGClient = null;
            Dictionary<string, TGClient> dic = TGClientManager.Instance.GetAllClient();
            foreach (KeyValuePair<string, TGClient> kv in dic)
            {
                tGClient = kv.Value;

                break;
            }

            return tGClient;
        }

        public void OnResult(TdApi.BaseObject result)
        {
            if (result != null)
            {
                //Print(result.ToString());
                Td.Api.Chats chats = result as Td.Api.Chats;
                if (chats != null)
                {
                    Task.Run(() =>
                    {
                        TGClient tGClient = GetOneClient();
                        foreach (long lon in chats.ChatIds)
                        {
                            if (lon < 0)
                            {
                                TdApi.GetChat getChat = new TdApi.GetChat() { ChatId = lon };

                                tGClient.GetClient().Send(getChat, this);

                                Thread.Sleep(200);
                            }
                        }
                    });

                }
                else
                {
                    Td.Api.Chat chat = result as Td.Api.Chat;
                    if (chat != null)
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            TdGroupInfo tdGroupInfo = new TdGroupInfo();
                            tdGroupInfo.GroupId = chat.Id;
                            tdGroupInfo.GroupName = chat.Title;

                            this.groupInfoList.Add(tdGroupInfo);
                        }));


                    }
                    //else
                    //{
                    //    TdApi.Message replayMsg = result as TdApi.Message;
                    //    if (replayMsg != null)
                    //    {
                    //        txtMsgId = replayMsg.ChatId;

                    //        Task.Run(() =>
                    //        {
                    //            TGClient tGClient = GetOneClient();
                    //            this.SendImageMsg(tGClient, txtMsgId, null);
                    //        });
                    //    }

                    //}

                }
            }
        }
    }
}
