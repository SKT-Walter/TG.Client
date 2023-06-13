using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class BatchSendMsgHandler : Td.ClientResultHandler
    {
        private Td.Client _client = null;


        public void Init(Td.Client client)
        {
            _client = client;
        }

        public void SendBatchMsg(string users)
        {

        }


        public void OnResult(TdApi.BaseObject @object)
        {

        }
    }
}
