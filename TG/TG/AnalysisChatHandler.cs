using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void Init(Td.Client client)
        {
            _client = client;
        }

        public void GetChat()
        {
            _client.Send(new TdApi.GetChats(new TdApi.ChatListMain(), 200), this);
        }

        public void OnResult(TdApi.BaseObject baseObject)
        {
            if (baseObject != null)
            {
                UserHandler.Instance.PublishMsg("AnalysisChat resp:" + baseObject.ToString());
                if (!(baseObject is TdApi.Error))
                {
                }
            }
        }
    }
}
