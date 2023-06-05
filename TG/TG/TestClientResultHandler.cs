using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class TestClientResultHandler : Td.ClientResultHandler
    {
        public void OnResult(TdApi.BaseObject @object)
        {
            Console.WriteLine(@object.ToString());
            MessageBox.Show(@object.ToString());
            
        }
    }
}
