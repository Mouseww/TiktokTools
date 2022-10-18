using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public class FileHelper
    {
        /// <summary>
        /// 向txt文件中写入字符串
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="isClearOldText">是否清除旧的文本</param>
        public static void Wriete(string path,string value, bool isClearOldText = true)
        {
            //是否清空旧的文本
            if (isClearOldText)
            {
                //清空txt文件
                using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.SetLength(0);
                }
            }
            //写入内容
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(value);
            }
        }

        /// <summary>
        /// 读取txt文件，并返回文件中的内容
        /// </summary>
        /// <returns>txt文件内容</returns>
        public static string ReadTxTContent(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return null;
                }

                string s_con = string.Empty;
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        s_con += line;
                    }
                }
                return s_con;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
