using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class ParseGroupHandler : Td.ClientResultHandler
    {
        private TdClientHandler tdClientHandler;

        public ParseGroupHandler(TdClientHandler tdClientHandler)
        {
            this.tdClientHandler = tdClientHandler;
        }

        public void OnResult(TdApi.BaseObject @object)
        {
            //if (@object is TdApi.UpdateAuthorizationState)
            //{

            //    tdClientHandler.OnAuthorizationStateUpdated((@object as TdApi.UpdateAuthorizationState).AuthorizationState);
            //}
            //else
            {
                Console.WriteLine("ParseGroupHandler Unsupported update: " + @object);
            }
        }
    }
}
