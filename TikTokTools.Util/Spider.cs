using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public class Spider
    {
        private HtmlDocument htmlDocument;

        public Spider(string data, int type=0)
        {
            htmlDocument = GetHtmlDoc(data, type);
        }
        /// <summary>
        /// Get HtmlDocument From Url
        /// </summary>
        /// <param name="url"></param>
        public HtmlDocument GetHtmlDoc(string data, int type ) {
            if (type == 1)
            {
                var html = new HtmlDocument();
                //html.UserAgent = "user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";
                //html.UseCookies = true;
                // From Web
                html.LoadHtml(data);
                return html;
            }
            else {
                var html = new HtmlWeb();
                html.UserAgent = "user-agent=Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1";
                //html.UseCookies = true;
                // From Web
                return html.Load(data);

            }

        }
       
        /// <summary>
        /// Get HtmlNode List from xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public  HtmlNodeCollection GetNodeList(string xpath) {
            return htmlDocument.DocumentNode.SelectNodes(xpath);
        }
        /// <summary>
        /// Get HtmlNode from xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public HtmlNode GetNode(string xpath)
        {
            return GetNodeList(xpath).FirstOrDefault();
        }
        /// <summary>
        /// Get Attributes Value from xpath
        /// </summary>
        /// <param name="xpath"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public string GetAttr(string xpath,string attributes) {
            return GetNode(xpath).Attributes[attributes].Value;
        }
        
        /// <summary>
        /// Get Attributes Value from Node
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static string GetNodeAttr(HtmlNode htmlNode, string attributes)
        {
            return htmlNode.Attributes[attributes].Value;
        }
        public static HtmlDocument LoadHtml(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            return htmlDoc;
        }

    }
}
