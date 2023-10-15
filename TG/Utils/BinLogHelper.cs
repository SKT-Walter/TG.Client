using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Utils
{
    public static class BinLogHelper
    {
        public static void DisableBinLog()
        {
            string tdBinLogPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\tdlib\td.binlog";

            if (File.Exists(tdBinLogPath))
            {
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.Arguments = "/C echo "+" > " + tdBinLogPath;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.Start();
                        process.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    // 处理异常
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
