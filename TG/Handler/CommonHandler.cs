using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Handler
{
    public class CommonHandler
    {
        private static CommonHandler commonHandler = new CommonHandler();

        public static CommonHandler Instance { get { return commonHandler; } }

        private CommonHandler()
        {

        }

        public event Action<int, int> OnGetMembers;

        public void PublishMemberChange(int total, int oneNum)
        {
            OnGetMembers?.Invoke(total, oneNum);
        }

    }
}
