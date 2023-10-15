using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.BatchTG;

namespace TG.Client.Cache
{
    public class TGClientManager
    {
        private static TGClientManager tdClientHandler = new TGClientManager();

        public static TGClientManager Instance { get { return tdClientHandler; } }

        private TGClientManager()
        {

        }

        private Dictionary<string, TGClient> dic = new Dictionary<string, TGClient>();
        private object lockObj = new object();

        public int GetCount()
        {
            return dic.Count;
        }

        public void AddOrUpdate(TGClient client, string account)
        {
            lock (lockObj)
            {
                if (dic.ContainsKey(account))
                {
                    dic[account] = client;
                }
                else
                {
                    dic.Add(account, client);
                }
            }
        }

        public TGClient GetClientByAcc(string account)
        {
            lock (lockObj)
            {
                if (dic.ContainsKey(account))
                {
                    return dic[account];
                }
                else
                    return null;
            }
        }
    }
}
