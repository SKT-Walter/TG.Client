using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG.Client.Model;

using Td = Telegram.Td;
using TdApi = Telegram.Td.Api;

namespace TG.Client.TG
{
    public class MsgHandler
    {

        private static MsgHandler msgHandler = new MsgHandler();

        public static MsgHandler Instance { get { return msgHandler; } }

        private MsgHandler()
        {

        }

        private Dictionary<string, TdMsgPo> msgDic = new Dictionary<string, TdMsgPo>();
        private HashSet<long> idSet = new HashSet<long>();
        private object lockObj = new object();
        private long currentTimeFilter = 0;

        public bool ContainId(long userId)
        {
            return idSet.Contains(userId);
        }

        public HashSet<long> GetAllId()
        {
            return idSet;
        }

        public int AddOrUpdate(TdApi.Messages messages, TimeFilterType timeFilterType)
        {
            int lastMsgTime = 0;
            lock (lockObj)
            {
                GetTimeFilter(timeFilterType);
                foreach (TdApi.Message msg in messages.MessagesValue)
                {
                    TdMsgPo tdMsgPo = new TdMsgPo();
                    tdMsgPo.MsgId = msg.Id;
                    TdApi.MessageSenderUser messageSenderUser = msg.SenderId as TdApi.MessageSenderUser;
                    if (messageSenderUser != null)
                    {
                        tdMsgPo.UserId = messageSenderUser.UserId;
                    }
                    tdMsgPo.Time = msg.Date;

                    if (lastMsgTime == 0 || msg.Date < lastMsgTime)
                    {
                        lastMsgTime = msg.Date;
                    }


                    bool needAdd = false;
                    if (timeFilterType == TimeFilterType.OneDay)
                    {
                        if (tdMsgPo.Time >= currentTimeFilter)
                        {
                            needAdd = true;
                        }
                        //var toUnixTime = new DateTimeOffset(toDate.Date).ToUnixTimeSeconds();
                    }
                    else if (timeFilterType == TimeFilterType.SevenDay)
                    {
                        if (tdMsgPo.Time >= currentTimeFilter)
                        {
                            needAdd = true;
                        }
                    }
                    else if (timeFilterType == TimeFilterType.None)
                    {
                        needAdd = true;
                    }

                    if (needAdd && !idSet.Contains(tdMsgPo.UserId))
                    {
                        idSet.Add(tdMsgPo.UserId);
                    }
                }
            }

            return lastMsgTime;
        }


        public bool ContinueSearchHis(int lastTime, TimeFilterType timeFilterType)
        {
            bool result = true;

            if (timeFilterType == TimeFilterType.OneDay || timeFilterType == TimeFilterType.SevenDay)
            {
                if (currentTimeFilter > lastTime)
                {
                    result = false;
                }
            }

            return result;
        }


        private void GetTimeFilter(TimeFilterType timeFilterType)
        {
            int interval = 0;
            if (timeFilterType == TimeFilterType.OneDay)
            {
                interval = -1;
            }
            else if (timeFilterType == TimeFilterType.SevenDay)
            {
                interval = -7;
            }

            if (currentTimeFilter == 0 && interval != 0)
            {
                DateTimeOffset toDate = DateTimeOffset.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                DateTimeOffset fromDate = DateTimeOffset.Parse(DateTime.Now.AddDays(interval).ToString("yyyy-MM-dd"));

                var fromUnixTime = new DateTimeOffset(fromDate.Date).ToUnixTimeSeconds();

                currentTimeFilter = fromUnixTime;
            }
        }

    }
}
