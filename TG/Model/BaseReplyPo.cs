using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model
{
    public class BaseReplyPo
    {
        public string Code = "0000";
        public string Msg;
        public int? IsAll;
        public string userName;
        public bool IsSuccess()
        {
            return Code.Equals("0000");
        }
    }
}
