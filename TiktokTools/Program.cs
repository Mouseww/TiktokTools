using ICSharpCode.SharpZipLib.Zip;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiktokTools;
using TikTokTools.Util;

namespace TikTokTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string guid = FingerPrint.Value();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WebUtils webUtils = new WebUtils();
            var newVersion = webUtils.DoGet("http://douyin.fhcollege.com/api/TikTookVersion.php");
            var oldVersion = Application.ProductVersion.ToString();

            string filename = Environment.CurrentDirectory + "\\1.1.3_update.zip";
            if ( !File.Exists(filename))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile("http://douyin.fhcollege.com/api/1.1.3_update.zip", filename);
                UnZip(filename, Environment.CurrentDirectory);
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\TikTokTools.Update.exe");
                System.Environment.Exit(0);
            }
            if (newVersion != oldVersion)
            {
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\TikTokTools.Update.exe");
                System.Environment.Exit(0);
            }


            var getResult = webUtils.DoGet("http://www.fhcollege.com/api/api/TikTookLogin?guid=" + guid);
            if (getResult == "true")
            {
                Application.Run(new Form1());
            }
            else { Application.Run(new Login()); }
            //Application.Run(new Form1());
        }

        //===================================================解压用的是库函数
        /// <summary>  
        /// 功能：解压zip格式的文件。  
        /// </summary>  
        /// <param name="zipFilePath">压缩文件路径</param>  
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        /// <returns>解压是否成功</returns>  
        public static void UnZip(string zipFilePath, string unZipDir)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹  
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("/"))
                unZipDir += "/";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            using (var s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (directoryName != null && !directoryName.EndsWith("/"))
                    {
                    }
                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {

                            int size;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                
            }
        }
    }
}
