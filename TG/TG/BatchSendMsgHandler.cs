using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Model;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class BatchSendMsgHandler : Td.ClientResultHandler
    {
        private Td.Client _client = null;
        private SendMsgType curentSendMsgType = SendMsgType.None;
        public string SendMsg { get; set; }

        private static BatchSendMsgHandler batchSendMsgHandler = new BatchSendMsgHandler();

        public static BatchSendMsgHandler Instance { get { return batchSendMsgHandler; } }

        private BatchSendMsgHandler()
        {

        }

        public BatchSendMsgHandler(Td.Client client, SendMsgType sendMsgType, string msg)
        {
            _client = client;
            curentSendMsgType = sendMsgType;
            SendMsg = msg;
        }

        public void Init(Td.Client client)
        {
            _client = client;
        }

        public void SendBatchMsg(string users, string msg)
        {
            SendMsg = msg;
            string[] userArr = users.Split(new char[] { '\n' });

            foreach (string user in userArr)
            {
                long userId = MsgHandler.Instance.GetIdByName(user);
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
                        SendMessage(chat.Id, SendMsg);
                    }
                }
            }
            else if (curentSendMsgType == SendMsgType.SearchChat)
            {

            }

                
        }

        private void SendMessage(long chatId, string message)
        {
            // initialize reply markup just for testing
            TdApi.InlineKeyboardButton[] row = { new TdApi.InlineKeyboardButton("https://telegram.org?1", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?2", new TdApi.InlineKeyboardButtonTypeUrl()),
                new TdApi.InlineKeyboardButton("https://telegram.org?3", new TdApi.InlineKeyboardButtonTypeUrl()) };
            TdApi.ReplyMarkup replyMarkup = new TdApi.ReplyMarkupInlineKeyboard(new TdApi.InlineKeyboardButton[][] { row, row, row });

            TdApi.InputMessageContent content = new TdApi.InputMessageText(new TdApi.FormattedText(message, null), false, true);
            _client.Send(new TdApi.SendMessage(chatId, 0, 0, null, replyMarkup, content), this);
        }
    }
}
