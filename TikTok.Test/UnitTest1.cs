using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TikTokTools.Util;

namespace TikTok.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var p=Proxy.GetWebProxy();
            HttpHelper httpHelper = new HttpHelper();
            HttpItem httpItem_List = new HttpItem();
            httpItem_List.UserAgent = "user-agent=" + GetUA();
            httpItem_List.URL = "https://v.kuaishou.com/54AMVe";
            httpItem_List.WebProxy = p;
            Spider spider = new Spider(httpHelper.GetHtml(httpItem_List).Html, 0);
           
            var nodeList = spider.GetNodeList("//*[@id=\"body-share-user\"]//li[@class=\"photo\"]/a");
            
            var a = 1;
        }
        private string GetUA()
        {
            return string.Format("Mozilla/5.0 (iPhone; CPU iPhone OS {0}_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1", new Random().Next(10, 13));

        }
    }
}
