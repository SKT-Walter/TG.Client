using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.Handler
{
    public class CommonHandler
    {
        private static CommonHandler commonHandler = new CommonHandler();

        public static CommonHandler Instance { get { return commonHandler; } }

        private CommonHandler()
        {

        }

        public event Action<TdApi.User> OnUserChange;
        public event Action<int, int> OnGetMembers;
        public event Action OnStartAnalysis;

        public void PublishUser(TdApi.User user)
        {
            OnUserChange.Invoke(user);
        }

        public void PublishMemberChange(int total, int oneNum)
        {
            OnGetMembers?.Invoke(total, oneNum);
        }

        public void PublishStartAnalysis()
        {
            OnStartAnalysis?.Invoke();
        }

    }
}
