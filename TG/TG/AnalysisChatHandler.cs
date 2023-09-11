using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TG.Client.Handler;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class AnalysisChatHandler : Td.ClientResultHandler
    {
        private Td.Client _client = null;
        private static AnalysisChatHandler analysisChatHandler = new AnalysisChatHandler();

        public static AnalysisChatHandler Instance { get { return analysisChatHandler; } }

        private AnalysisChatHandler()
        {

        }

        private List<TdApi.Chat> chatList = new List<TdApi.Chat>();

        public List<TdApi.Chat> ChatList
        {
            get { return chatList; }
        }

        public void Init(Td.Client client)
        {
            _client = client;
        }

        public void GetChats()
        {
            _client.Send(new TdApi.GetChats(new TdApi.ChatListMain(), 200), this);
        }

        public void OnResult(TdApi.BaseObject baseObject)
        {
            try
            {
                if (baseObject != null)
                {
                    UserHandler.Instance.PublishMsg("AnalysisChat resp:" + baseObject.ToString());
                    if (!(baseObject is TdApi.Error))
                    {
                        if (baseObject is TdApi.Chats)
                        {
                            TdApi.Chats chats = baseObject as TdApi.Chats;

                            GetChatListDetail(chats);
                        }
                        else if (baseObject is TdApi.Chat)
                        {
                            TdApi.Chat chat = baseObject as TdApi.Chat;
                            if (chat.Type is TdApi.ChatTypeSupergroup)
                            {
                                chatList.Add(baseObject as TdApi.Chat);
                                UserHandler.Instance.PublishMsg("Add chat size:" + chatList.Count);
                                UserHandler.Instance.PublishMsg("Add chat:" + baseObject.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UserHandler.Instance.PublishMsg("Analysis OnResult exeception:" + e.Message);
            }
        }

        private void GetChatListDetail(TdApi.Chats chats)
        {
            Task.Run(() =>
            {
                foreach (long chatId in chats.ChatIds)
                {
                    _client.Send(new TdApi.GetChat(chatId), this);

                    Thread.Sleep(100);
                }

                Thread.Sleep(1000);
                CommonHandler.Instance.PublishStartAnalysis();
            });
        }
    }
}
