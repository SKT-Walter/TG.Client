using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;
using TG.Client.Utils;
using TG.Client.Utils.SqlLite;

namespace TG.Client.Handler
{
    public class UserHandler
    {
        private bool isInit = true;
        private UserHandler() 
        {
        }
        private static UserHandler server = new UserHandler();

        public event Action<object> OnChange;
        public static UserHandler Instance { get { return server; } }


        private AsyncThreadQueue<TdUserPo> userThread;

        public void PublishMsg(object obj)
        {
            OnChange?.Invoke(obj);
        }

        public void Init()
        {
            if (isInit)
            {
                SqliteManager.Initialize();

                List<string> pkColList = new List<string>();
                pkColList.Add("UserId");
                SqlliteUtils.CreateTable<TdUserPo>(pkColList);

                userThread = new AsyncThreadQueue<TdUserPo>(UpdateData);

                isInit = false;
            }
        }

        private void UpdateData(TdUserPo userPo)
        {
            TdUserPo dbUser = QuoteUserById(userPo.UserId);
            if (dbUser != null)
            {
                if (string.IsNullOrEmpty(dbUser.Flag) || !dbUser.Flag.Contains(userPo.Flag))
                {
                    dbUser.Flag += userPo.UserId + ",";
                }
            }
            else
            {
                dbUser = userPo;
            }
            SqlliteUtils.Replace<TdUserPo>(dbUser);
        }

        public void SaveUser(TdUserPo po)
        {
            userThread.Enqueue(po);


        }

        public TdUserPo QuoteUserByName(string name)
        {
            try
            {
                List<TdUserPo> list = SqlliteUtils.Query<TdUserPo>
                    ("select * from TdUserPo where Name=?",
                    new object[] { name });
                if (list.Count > 0)
                {
                    return list[0];
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public TdUserPo QuoteUserById(long userId)
        {
            try
            {
                List<TdUserPo> list = SqlliteUtils.Query<TdUserPo>
                    ("select * from TdUserPo where UserId=?",
                    new object[] { userId });
                if (list.Count > 0)
                {
                    return list[0];
                }
            }
            catch (Exception e)
            {
            }
            return null;
        }
    }
}
