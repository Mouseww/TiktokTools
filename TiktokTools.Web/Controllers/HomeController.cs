using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TikTokTools.Util;
using static TikTokTools.Util.ConvertHelperCmd;

namespace TiktokTools.Web.Controllers
{
    public class HomeController : Controller
    {
        CancellationTokenSource cts;
        CancellationToken ct
        {
            get { return cts.Token; }
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public void RunLog(string msg)
        {
            Console.WriteLine(msg);
        }

        [HttpPost]
        public HttpResponseMessage AdjustVideo()
        {
            var file = Request.Files[0];  //获取文件对象
            string fileName = file.FileName;  //原文件名
            string fileExt = fileName.Substring(fileName.LastIndexOf("."));//获取扩展名

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExt; //随机生成新的文件名
            string path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource"), newFileName);  //生成新的文件路径
            file.SaveAs(path);  //保存文件
            var config = new ConfigEntity();
            config.LocalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            config.SourcePath = System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource") + "\\" + newFileName;
            cts = new CancellationTokenSource();
            var result = new List<string>();
            Run log = new Run(RunLog);
            Run changestatus = new Run(RunLog);
            try
            {
                var resultdata = new ConvertHelperCmd().Convert(config, log, ct, changestatus);
                foreach (var item in resultdata)
                {
                    var array = item.Split('\\');
                    result.Add(Request.Url.Host + "/Finish/" + array[array.Length - 1]);
                }
            }
            catch (Exception ex)
            {
                log("程序已停止:" + ex.Message);
                changestatus("Stop");
            }
            return new HttpResponseMessage
            {
                Content = new StringContent(string.Join(",", result), Encoding.GetEncoding("UTF-8"), "application/json")
            };
            //return new FileInfo();
        }

    }
}
