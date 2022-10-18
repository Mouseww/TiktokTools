using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TikTokTools.Util;
using static TikTokTools.Util.ConvertHelperCmd;

namespace TiktokTools.Web.Controllers
{
    public class ConvertController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        CancellationTokenSource cts;
        CancellationToken ct
        {
            get { return cts.Token; }
        }

        public void RunLog(string msg, string userId = null) {
            Console.WriteLine(msg);
        }

        [HttpPost]
        public HttpResponseMessage AdjustVideo()
        {
            var file = HttpContext.Current.Request.Files[0];  //获取文件对象
            string fileName = file.FileName;  //原文件名
            string fileExt = fileName.Substring(fileName.LastIndexOf("."));//获取扩展名
                          
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + fileExt; //随机生成新的文件名
            string path = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource"), newFileName);  //生成新的文件路径
            file.SaveAs(path);  //保存文件
            var config = new ConfigEntity();
            config.LocalPath = System.Web.Hosting.HostingEnvironment.MapPath("~/");
            config.SourcePath = System.Web.Hosting.HostingEnvironment.MapPath("~/TempSource")+"\\"+newFileName;
            cts = new CancellationTokenSource();
            var result = new List<string>();
            Run log = new Run(RunLog);
            Run changestatus = new Run(RunLog);
            try
            {
                var resultdata = new ConvertHelperCmd().ConvertAsync(config, log, ct, changestatus).GetAwaiter().GetResult();
                foreach (var item in resultdata)
                {
                    var array = item.Split('\\');
                    result.Add(Url.Request.Headers.Host+"/Finish/" + array[array.Length - 1]);
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
