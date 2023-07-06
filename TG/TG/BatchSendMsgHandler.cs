using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Handler;
using TG.Client.Model;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class BatchSendMsgHandler : Td.ClientResultHandler
    {
        private long txtMsgId = 0;
        private Td.Client _client = null;
        private TdApi.Chat txtMsgChat = null;
        private SendMsgType curentSendMsgType = SendMsgType.None;
        public SendMsgPo SendMsg { get; set; }

        private static BatchSendMsgHandler batchSendMsgHandler = new BatchSendMsgHandler();

        public static BatchSendMsgHandler Instance { get { return batchSendMsgHandler; } }

        private BatchSendMsgHandler()
        {

        }

        public BatchSendMsgHandler(Td.Client client, SendMsgType sendMsgType, SendMsgPo msg, TdApi.Chat chat = null)
        {
            _client = client;
            curentSendMsgType = sendMsgType;
            SendMsg = msg;

            if (chat != null)
            {
                txtMsgChat = chat;
            }
        }

        public void Init(Td.Client client)
        {
            _client = client;
        }

        public void SendBatchMsg(string users, SendMsgPo sendMsgPo)
        {
            SendMsg = sendMsgPo;
            string[] userArr = users.Split(new char[] { '\n' });

            foreach (string user in userArr)
            {
                long userId = 0;
                string name = user.Replace("@", "").Replace("\r", "");
                TdUserPo userPo = UserHandler.Instance.QuoteUserByName(name);
                if (userPo != null)
                {
                    userId = userPo.UserId;
                }

                UserHandler.Instance.PublishMsg("------尝试发送消息到用户：" + name + ", userId:" + userId);
                //MsgHandler.Instance.GetIdByName(user);
                if (userId != 0)
                {
                    _client.Send(new TdApi.CreatePrivateChat() { UserId = userId, Force = false }, new BatchSendMsgHandler(_client, SendMsgType.CreateChat, SendMsg));


                    //_client.Send(new TdApi.SearchPublicChat() { Username = user }, new BatchSendMsgHandler(_client, SendMsgType.SearchChat, SendMsg));
                }
                Thread.Sleep(100);
            }
        }


        public void OnResult(TdApi.BaseObject baseObject)
        {
            Console.WriteLine("BatchSendMsgHandler:" + baseObject.ToString());
            UserHandler.Instance.PublishMsg(baseObject.ToString());
            if (baseObject != null && !(baseObject is TdApi.Error))
            {
                if (curentSendMsgType == SendMsgType.SearchChat)
                {
                    TdApi.Chat chat = baseObject as TdApi.Chat;
                    if (chat != null)
                    {
                        long userId = chat.Id;

                        _client.Send(new TdApi.CreatePrivateChat() { UserId = userId, Force = false }, new BatchSendMsgHandler(_client, SendMsgType.CreateChat, SendMsg));
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

        private void SendMessage(long chatId, SendMsgPo message)
        {
            // initialize reply markup just for testing
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };

            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });

            if (!string.IsNullOrEmpty(message.SendMsg))
            {
                TdApi.InputMessageContent content = new TdApi.InputMessageText(new TdApi.FormattedText(message.SendMsg, null), false, true);

                _client.Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, content),
                    !string.IsNullOrEmpty(message.FilePath) ? new BatchSendMsgHandler(_client, SendMsgType.SendTxtAndImage, SendMsg, txtMsgChat) : this);
            }
            
        }

        private void SendImageMsg(long chatId, SendMsgPo message)
        {
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };

            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });
            
            if (!string.IsNullOrEmpty(message.FilePath))
            {
                // 发送图片消息

                TdApi.InputMessageContent photoMessage = new TdApi.InputMessagePhoto
                {
                    Photo = new TdApi.InputFileLocal(@message.FilePath),//(@"C:\photos\my-photo.jpg"),  // 这里你需要使用实际图片路径
                    Caption = new TdApi.FormattedText { Text = string.Empty },//"图片说明" },  //图片下面会显示的文字
                    
                };
                
                _client.Send(new TdApi.SendMessage(chatId, 0, txtMsgId, null, replyMarkup, photoMessage), this);
            }
        }
    }
}
