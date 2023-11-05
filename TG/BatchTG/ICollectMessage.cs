using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;

namespace TG.Client.BatchTG
{
    public interface ICollectMessage
    {
        void Instance_OnUserChange(Telegram.Td.Api.User user);
        void Instance_OnBotUserChange(Telegram.Td.Api.User obj);
        void Instance_OnGetMembers(int arg1, int arg2);
        
    }
}
