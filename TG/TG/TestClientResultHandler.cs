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
        public void OnResult(TdApi.BaseObject reps)
        {
            Console.WriteLine(reps.ToString());
            MessageBox.Show(reps.ToString());

            if (reps is TdApi.Chats)
            {
                TdApi.Chats chats = reps as TdApi.Chats;
                
                foreach (long chatId in chats.ChatIds)
                {
                    if (chatId > 0)
                    {
                        TdApi.GetChat getChat = new TdApi.GetChat() { ChatId = chatId };
                    }
                }
            }

            
        }
    }
}
