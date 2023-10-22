using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Client.Utils
{
    public class FileHelper
    {
        public static string[] ReadFile(string filePath)
        {
            // 读取文件内容
            string[] lines = File.ReadAllLines(filePath);

            return lines;
        }
    }
}
