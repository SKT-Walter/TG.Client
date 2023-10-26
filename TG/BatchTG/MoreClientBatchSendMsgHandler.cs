using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Cache;
using TG.Client.Handler;
using TG.Client.Model;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.BatchTG
{
    public class MoreClientBatchSendMsgHandler : Td.ClientResultHandler
    {
        private long txtMsgId = 0;
        private string _currentAcc = string.Empty;
        private TdApi.Chat txtMsgChat = null;
        private SendMsgType curentSendMsgType = SendMsgType.None;
        public SendMsgPo SendMsg { get; set; }

        //private static MoreClientBatchSendMsgHandler batchSendMsgHandler = new MoreClientBatchSendMsgHandler();

        //public static MoreClientBatchSendMsgHandler Instance { get { return batchSendMsgHandler; } }

        //private MoreClientBatchSendMsgHandler()
        //{

        //}

        public MoreClientBatchSendMsgHandler(string account, SendMsgType sendMsgType, SendMsgPo msg, TdApi.Chat chat = null)
        {
            this._currentAcc = account;
            curentSendMsgType = sendMsgType;
            SendMsg = msg;

            if (chat != null)
            {
                txtMsgChat = chat;
            }
        }

        public void Reset(string account, SendMsgType sendMsgType, SendMsgPo msg, TdApi.Chat chat = null)
        {
            this._currentAcc = account;
            curentSendMsgType = sendMsgType;
            SendMsg = msg;

            if (chat != null)
            {
                txtMsgChat = chat;
            }
        }


        public void SendBatchMsg(string users, SendMsgPo sendMsgPo, int startInterval, int endInterval)
        {
            SendMsg = sendMsgPo;
            string[] userArr = users.Split(new char[] { '\n' });
            UserHandler.Instance.PublishMsg("TGClient:" + _currentAcc + ", start send msg...");
            Random random = new Random();
            Td.Client _client = TGClientManager.Instance.GetClientByAcc(_currentAcc).GetClient();
            if (_client != null)
            {
                Task.Run(() =>
                {
                    foreach (string user in userArr)
                    {
                        long userId = 0;
                        string name = user.Replace("@", "").Replace("\r", "");
                        TdUserPo userPo = UserHandler.Instance.QuoteUserExByName(name);
                        if (userPo != null)
                        {
                            userId = userPo.UserId;
                        }

                        UserHandler.Instance.PublishMsg("------尝试发送消息到用户：" + name + ", userId:" + userId);
                        //MsgHandler.Instance.GetIdByName(user);
                        if (userId != 0)
                        {
                            _client.Send(new TdApi.CreatePrivateChat() { UserId = userId, Force = true }, new MoreClientBatchSendMsgHandler(_currentAcc, SendMsgType.CreateChat, SendMsg));

                            //_client.Send(new TdApi.SearchPublicChat() { Username = user }, new BatchSendMsgHandler(_client, SendMsgType.SearchChat, SendMsg));
                        }
                        int interval = random.Next(startInterval, endInterval) * 1000;
                        UserHandler.Instance.PublishMsg("下次发送将等待：" + (interval / 1000) + "秒");
                        Thread.Sleep(interval);
                    }
                });
            }
            else
            {
                UserHandler.Instance.PublishMsg("通过账号：" + _currentAcc + ", 获取TGClient失败");
            }
        }


        public void OnResult(TdApi.BaseObject baseObject)
        {
            //Console.WriteLine("BatchSendMsgHandler:" + baseObject.ToString());
            if (baseObject != null)
            {
                UserHandler.Instance.PublishMsg("resp:" + baseObject.ToString());
                if (!(baseObject is TdApi.Error))
                {
                    if (curentSendMsgType == SendMsgType.SearchChat)
                    {
                        TdApi.Chat chat = baseObject as TdApi.Chat;
                        if (chat != null)
                        {
                            long userId = chat.Id;

                            Td.Client _client = TGClientManager.Instance.GetClientByAcc(_currentAcc).GetClient();
                            if (_client != null)
                            {

                                _client.Send(new TdApi.CreatePrivateChat() { UserId = userId, Force = true }, new MoreClientBatchSendMsgHandler(_currentAcc, SendMsgType.CreateChat, SendMsg));
                            }
                            else
                            {
                                UserHandler.Instance.PublishMsg("通过账号：" + _currentAcc + ", 获取TGClient失败");
                            }
                        }
                    }
                    else if (curentSendMsgType == SendMsgType.CreateChat)
                    {
                        TdApi.Chat chat = baseObject as TdApi.Chat;
                        if (chat != null)
                        {
                            txtMsgChat = chat;
                            SendMessage(chat.Id, SendMsg);
                        }
                    }
                    else if (curentSendMsgType == SendMsgType.SendTxtAndImage)
                    {
                        TdApi.Message replayMsg = baseObject as TdApi.Message;
                        if (replayMsg != null)
                        {
                            txtMsgId = replayMsg.ChatId;

                            SendImageMsg(txtMsgChat.Id, SendMsg);

                            this.curentSendMsgType = SendMsgType.None;
                        }
                    }

                }
            }
        }

        private void SendMessage(long chatId, SendMsgPo message)
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

                Td.Client _client = TGClientManager.Instance.GetClientByAcc(_currentAcc).GetClient();
                if (_client != null)
                {

                    _client.Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, content),
                        !string.IsNullOrEmpty(message.FilePath) ? new MoreClientBatchSendMsgHandler(_currentAcc, SendMsgType.SendTxtAndImage, SendMsg, txtMsgChat) : this);
                }
                else
                {
                    UserHandler.Instance.PublishMsg("通过账号：" + _currentAcc + ", 获取TGClient失败");
                }
            }

        }

        private void SendImageMsg(long chatId, SendMsgPo message)
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
                Td.Client _client = TGClientManager.Instance.GetClientByAcc(_currentAcc).GetClient();
                if (_client != null)
                {
                    _client.Send(new TdApi.SendMessage(chatId, 0, txtMsgId, null, replyMarkup, photoMessage), this);
                }
                else
                {
                    UserHandler.Instance.PublishMsg("通过账号：" + _currentAcc + ", 获取TGClient失败");
                }
            }
        }
    }
}
