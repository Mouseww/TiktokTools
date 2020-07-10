using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TikTokTools.Update
{
    public partial class Form_Update : Form
    {
        public Form_Update()
        {
            InitializeComponent();
        }

        private void Form_Update_Load(object sender, EventArgs e)
        {
            WebUtils webUtils = new WebUtils();
            var newVersion = webUtils.DoGet("http://douyin.fhcollege.com/api/TikTookVersion.php");
            WebClient webClient = new WebClient();
            string filename = Environment.CurrentDirectory + "\\" + newVersion + ".zip";
            webClient.DownloadFile("http://douyin.fhcollege.com/api/" + newVersion + ".zip", filename);
            UnZip(filename, Environment.CurrentDirectory);
        }

        //===================================================解压用的是库函数
        /// <summary>  
        /// 功能：解压zip格式的文件。  
        /// </summary>  
        /// <param name="zipFilePath">压缩文件路径</param>  
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>  
        /// <returns>解压是否成功</returns>  
        public  void UnZip(string zipFilePath, string unZipDir)
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
                    try
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
                                        progressBar1.Value = (int)(streamWriter.Length / s.Length) * 100;
                                        label1.Text = progressBar1.Value + "%";
                                    }
                                    else
                                    {
                                        label1.Text = "更新完成";

                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch {

                    }
                }

                System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\TiktokTools.exe");
                this.Close();
            }
        }
    }
}
