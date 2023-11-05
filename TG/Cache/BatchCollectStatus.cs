using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Cache
{
    public class BatchCollectStatus
    {
        private static BatchCollectStatus server = new BatchCollectStatus();

        public static BatchCollectStatus Instance { get { return server; } }

        private BatchCollectStatus()
        {
        }

        private Dictionary<string, bool> statusDic = new Dictionary<string, bool>();

        public void AddOrUpdate(string acc, bool status)
        {
            if (statusDic.ContainsKey(acc))
            {
                statusDic[acc] = status;
            }
            else
            {
                statusDic.Add(acc, status);
            }
        }

        public void Clear()
        {
            statusDic.Clear();
        }

        public bool AllIsOk()
        {
            bool result = true;

            foreach (KeyValuePair<string, bool> kv in statusDic)
            {
                if (!kv.Value)
                {
                    result = false;

                    break;
                }
            }

            return result;
        }
    }
}
