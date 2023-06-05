using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    class DefaultHandler : Td.ClientResultHandler
    {
        void Td.ClientResultHandler.OnResult(TdApi.BaseObject @object)
        {
            Console.WriteLine(@object.ToString());
        }

    }
}
