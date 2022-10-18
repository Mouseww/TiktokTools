using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using TikTokTools.Util;
using TikTokTools.Util.SpiderVideo;
using WebMatrix.WebData;
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
            ViewBag.UserID = Guid.NewGuid().ToString().ToLower();
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Link()
        {
            ViewBag.UserID = Guid.NewGuid().ToString().ToLower();
            ViewBag.Title = "Home Page";
            return View();
        }

        public void RunLog(string msg, string userId = null)
        {
            if (userId == null)
            {
                return;
            }

            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            _hubContext.Clients.Client(userId).addNewMessageToPage(msg);
        }


        public void ResponseLink(string link, string userId = null)
        {
            if (userId == null)
            {
                return;
            }

            IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            _hubContext.Clients.Client(userId).adjustSuccess(link);
        }

        [HttpPost]
        public JsonResult AdjustVideo(HttpPostedFileBase file, bool mirror, string userId, int cropType)
        {
            string fileName = file.FileName;  //原文件名
            string fileExt = fileName.Substring(fileName.LastIndexOf("."));//获取扩展名

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExt; //随机生成新的文件名
            string path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource"), newFileName);  //生成新的文件路径
            file.SaveAs(path);  //保存文件
            runDistinct(path, mirror, userId, cropType);

            return Json(new { Content = "success"});
                //return new FileInfo();
            }


        [HttpPost]
        public JsonResult AdjustVideoFromLink(string link, bool mirror, string userId, int cropType)
        {
            SpiderVideoHelper spiderVideoHelper = new SpiderVideoHelper();

            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".mp4"; //随机生成新的文件名
            string path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource"), newFileName);  //生成新的文件路径
            RunLog("正在无水印解析视频",userId);
            spiderVideoHelper.GetVideo(link, path);
            runDistinct(path,mirror,userId,cropType);

            return Json(new { Content = "success" });
            //return new FileInfo();
        }

        private void runDistinct(string path,bool mirror, string userId, int cropType)
        {
            Task.Run(() =>
            {
                Run log = new Run(RunLog);
                Run changestatus = new Run(RunLog);

                log(" Run Start");
                //var file = Request.Files[0];  //获取文件对象

                var config = new ConfigEntity();
                config.LocalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
                config.SourcePath = path;
                config.Video_Mirroring = mirror;
                config.UserID = userId;
                config.CropType = cropType;
                cts = new CancellationTokenSource();
                var result = new List<string>();
                try
                {
                    var resultdata = new ConvertHelperCmd().ConvertAsync(config, log, ct, changestatus).GetAwaiter().GetResult();
                    foreach (var item in resultdata)
                    {
                        var array = item.Split('\\');
                        if (Request.Url.AbsoluteUri.IndexOf("localhost") > -1)
                        {
                            result.Add(Request.Url.AbsoluteUri.Replace(Request.Url.AbsolutePath, "/Finish") + "/" + array[array.Length - 1]);
                            continue;
                        }

                        result.Add("http://" + Request.Url.Host + "/Finish/" + array[array.Length - 1]);
                    }
                }
                catch (Exception ex)
                {
                    log("程序已停止:" + ex.Message);
                    changestatus("Stop");
                }

                log(" End");
                ResponseLink(result[0], userId);
            });
        }

        private VideoSource GetVideoSource(string link)
        {
            throw new NotImplementedException();
        }
    }
}
