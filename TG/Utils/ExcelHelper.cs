
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TG.Client.Model;

namespace TG.Client.Utils
{
    /// <summary>
    /// 导出CSV工具类
    /// </summary>
    public class ExcelHelper
    {
        private static ExcelHelper hepler = new ExcelHelper();
        public static ExcelHelper Instance { get { return hepler; } }

        private string msg = "";

        public string Msg
        {
            get
            {
                return msg;
            }

            set
            {
                msg = value;
            }
        }
        private Dictionary<string, int> columnDic = new Dictionary<string, int>();
        private Regex regNum = new Regex("[\u4e00-\u9fbb]");
        /// <summary>
        /// Save the List data to CSV file
        /// </summary>
        /// <param name="obj">data source</param>
        /// <param name="filePath">file path</param>
        /// <returns>success flag</returns>
        public bool SaveDataToCSVFile(ExportPo obj, string filePath, StreamWriter sw ,int count,string uiName)
        {
            bool successFlag = true;

            StringBuilder strColumn = new StringBuilder();
            StringBuilder strValue = new StringBuilder();
            PropertyInfo[] props = GetPropertyInfoArray();

            try
            {
                if (count == 0)
                {
                    columnDic.Clear();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    //if (uiName.Equals(Constants.HISOrders))
                    //{
                    //    dic = TableHeaderUtils.Instance.OrderTableHeader;
                    //}
                    //else if (uiName.Equals(Constants.HISEMSOrders))
                    //{
                    //    dic = TableHeaderUtils.Instance.EMSOrderTableHeader;
                    //}
                    //else if (uiName.Equals(Constants.HISDeals)) 
                    //{
                    //    dic = TableHeaderUtils.Instance.DealsTableHeader;
                    //}
                    //else if (uiName.Equals(Constants.HISRisks)) 
                    //{
                    //    dic = TableHeaderUtils.Instance.RiskHistoryTableHeader;
                    //}
                    //else if (uiName.Equals(Constants.HISMsgs))
                    //{
                    //    dic = TableHeaderUtils.Instance.HistoryMsgTableHeader;
                    //}
                    //else if (uiName.Equals(Constants.HISAgens))
                    //{
                    //    dic = TableHeaderUtils.Instance.HistoryAgencyTableHeader;
                    //}
                    foreach (KeyValuePair<string, string> kvp in dic)
                    {
                        for (int i = props.Length - 1; i >= 0; i--)
                        {
                            string name = props[i].Name;
                            if (kvp.Key.Equals(name))
                            {
                                strColumn.Append(kvp.Value);
                                strColumn.Append(",");
                                columnDic.Add(name,i);
                            }
                        }
                    }                               
                    strColumn.Remove(strColumn.Length - 1, 1);
                    sw.WriteLine(strColumn);    //write the column name
                }
                //foreach(KeyValuePair<string,int> kvp in columnDic)
                //{
                //    var itemPropery = props[kvp.Value];
                //    var val = itemPropery.GetValue(obj, null);
                //    if (TableHeaderUtils.Instance.ChineseDic.ContainsKey(kvp.Key) && val != null && !"".Equals(val))
                //    {
                //        strValue.Append("\"" + val + "\"");
                //    }
                //    else
                //    {
                //        if (TableHeaderUtils.Instance.SignDic.ContainsKey(kvp.Key) && val != null && !"".Equals(val))
                //        {
                //            strValue.Append(val+ "\t");
                //        }
                //        else
                //        {
                //            strValue.Append(val);
                //        }
                //    }
                //    strValue.Append(",");
                //}
                strValue.Remove(strValue.Length - 1, 1);
                sw.WriteLine(strValue); //write the row value
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                successFlag = false;
            }
            finally
            {
                
            }

            return successFlag;
        }
        public StreamWriter createStream(string fileName)
        {
            FileStream fs;
            StreamWriter sw;
            try
            {
                string savePath = Path.GetDirectoryName(fileName);
                string saveFile = fileName;
                //创建路径和文件
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }
                if (!File.Exists(saveFile))
                {
                    fs = new FileStream(saveFile, FileMode.CreateNew, FileAccess.ReadWrite);
                }
                else
                {
                    File.Delete(saveFile);
                    fs = new FileStream(saveFile, FileMode.CreateNew, FileAccess.ReadWrite);
                }
                sw = new StreamWriter(fs, Encoding.UTF8);
                return sw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
       
      

        #region common
        private PropertyInfo[] GetPropertyInfoArray()
        {
            PropertyInfo[] props = null;
            try
            {
                Type type = typeof(ExportPo);
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            catch (Exception ex)
            { }
            return props;
        }
       
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="FILE_NAME">文件路径</param>
        /// <returns>文件的编码类型</returns>

        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
            System.IO.FileAccess.Read);
            System.Text.Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetType(System.IO.FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            System.Text.Encoding reVal = System.Text.Encoding.Default;

            System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = System.Text.Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = System.Text.Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = System.Text.Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        /// <summary>
        /// 修改文件名称
        /// 我们需要保存历史数据 或者实时的知道那个文件被修改 可以通过改变文件的名称 如加上当天的日期等等。
        /// </summary>
        /// <param name="OldPath"></param>
        /// <param name="NewPath"></param>
        /// <returns></returns>
        public static bool ChangeFileName(string OldPath, string NewPath)
        {
            bool re = false;
            try
            {
                if (File.Exists(OldPath))
                {
                    File.Move(OldPath, NewPath);
                    re = true;
                }
            }
            catch
            {
                re = false;
            }
            return re;
        }

        /// <summary>
        /// CSV文件的数据写入
        /// 直接在网页表单提交数据保存在csv文件中 直接写入文件
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static bool SaveCSV(string fullPath, string Data)
        {
            bool re = true;
            try
            {
                FileStream FileStream = new FileStream(fullPath, FileMode.Append);
                StreamWriter sw = new StreamWriter(FileStream, System.Text.Encoding.UTF8);
                sw.WriteLine(Data);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                FileStream.Close();
            }
            catch
            {
                re = false;
            }
            return re;
        }
        #endregion
    }
}
