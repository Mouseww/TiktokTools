using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public static class Proxy
    {
        public static WebProxy GetWebProxy() {
            Spider spider = new Spider("https://www.7yip.cn/free/");
            var nodeList=spider.GetNodeList("//*[@id=\"content\"]//tbody/tr");
            Random random = new Random();
            List<ProxyInfo> ts = new List<ProxyInfo>();
            foreach (var item in nodeList)
            {
                var tds = item.ChildNodes.Where(x=>x.Name=="td").ToList();
                if (tds[3].InnerHtml=="HTTPS") {
                    var p = new ProxyInfo(tds[0].InnerHtml, tds[1].InnerHtml);
                    var proxy = new WebProxy(p.address, p.port);
                    HttpWebRequest Req = (HttpWebRequest)WebRequest.Create("https://www.baidu.com");
                    Req.Proxy = proxy; //设置代理
                    Req.Method = "GET";
                    HttpWebResponse Resp = (HttpWebResponse)Req.GetResponse();
                    ts.Add(p);
                }
            }
            var obj = ts[random.Next(ts.Count)];
            
            return new WebProxy(obj.address, obj.port);
            //var item=obj.InnerHtml.Replace("<br>","|").Split('|')[1].Split(':');
            //return new WebProxy(item[0].ToString(),Int32.Parse( item[1].ToString()));
        }
    }

    public class ProxyInfo
    {
        public ProxyInfo(string address, string port) {
            this.address = address;
            this.port =Convert.ToInt32( port.Trim());
        }
        public string address { get; set; }

        public int port { get; set; }
    }
}
