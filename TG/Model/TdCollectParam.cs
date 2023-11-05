using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model
{
    public class TdCollectParam
    {
        private string account;
        private OperatorType currentOperatorType = OperatorType.SearchChat;
        private long currentChatId = 0;
        private long currentGruopId = 0;
        private long currentChatLastMsgId = 0;
        private int startIndex = 0;
        private int endIndex = 0;
        private TimeFilterType currentTimeFilterType = TimeFilterType.None;
        private int currentCollectNum = 0;
        private int limit = 200;
        private string currentGroupName = string.Empty;
        private string currentGroupUrl = string.Empty;
        private bool saveIntoDB = false;



        public long CurrentChatId { get => currentChatId; set => currentChatId = value; }
        public OperatorType CurrentOperatorType { get => currentOperatorType; set => currentOperatorType = value; }
        public long CurrentGruopId { get => currentGruopId; set => currentGruopId = value; }
        public long CurrentChatLastMsgId { get => currentChatLastMsgId; set => currentChatLastMsgId = value; }
        public int StartIndex { get => startIndex; set => startIndex = value; }
        public int EndIndex { get => endIndex; set => endIndex = value; }
        public TimeFilterType CurrentTimeFilterType { get => currentTimeFilterType; set => currentTimeFilterType = value; }
        public int CurrentCollectNum { get => currentCollectNum; set => currentCollectNum = value; }
        public string CurrentGroupName { get => currentGroupName; set => currentGroupName = value; }
        public int Limit { get => limit; set => limit = value; }
        public string CurrentGroupUrl { get => currentGroupUrl; set => currentGroupUrl = value; }
        public string Account { get => account; set => account = value; }
        public bool SaveIntoDB { get => saveIntoDB; set => saveIntoDB = value; }
    }
}
