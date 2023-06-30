using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Model
{
    public class SendMsgPo
    {
        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;

            }
        }

        private string sendMsg;
        public string SendMsg
        {
            get { return sendMsg; }
            set
            {
                sendMsg = value;

            }
        }
    }
}
