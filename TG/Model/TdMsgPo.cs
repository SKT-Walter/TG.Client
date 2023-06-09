using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model
{
    public class TdMsgPo : TdUserPo
    {
        public long MsgId { get; set; }
        
        public int Time { get; set; }
    }
}
