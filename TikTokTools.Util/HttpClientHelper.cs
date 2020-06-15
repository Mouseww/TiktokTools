using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace TikTokTools.Util
{
    public class HttpClientHelper
    {
        /// <summary>
        /// post数据
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="postParaList"></param>
        /// <param name="isFile">true:用multipart/form-data发送，false：默认格式</param>
        /// <returns></returns>
        public static string postMessage(string strUrl, List<PostDateClass> postParaList, bool isFile = false,string method="Post")
        {
            if (isFile == true)
            {
                return postFileMessage(strUrl, postParaList);
            }
            else
            {
                StringBuilder strPost = new StringBuilder();
                for (int i = 0; i < postParaList.Count; i++)
                {
                    if (i != 0)
                    {
                        strPost.Append("&");
                    }
                    strPost.Append(postParaList[i].Prop);
                    strPost.Append("=");
                    strPost.Append(postParaList[i].Value);
                }
                return postMessage(strUrl, strPost.ToString());
            }
        }
        public static string postFileMessage(string strUrl, List<PostDateClass> postParaList)
        {
            try
            {
                string responseContent = "";
                var memStream = new MemoryStream();
                var webRequest = (HttpWebRequest)WebRequest.Create(strUrl);
                // 边界符
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                // 边界符
                var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                // 最后的结束符
                var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                // 设置属性
                webRequest.Method = "POST";
                webRequest.Timeout = 10000;
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                for (int i = 0; i < postParaList.Count; i++)
                {
                    PostDateClass temp = postParaList[i];
                    if (temp.Type == 1)
                    {
                        var fileStream = new FileStream(temp.Value, FileMode.Open, FileAccess.Read);
                        // 写入文件
                        const string filePartHeader =
                            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                            "Content-Type: application/octet-stream\r\n\r\n";
                        var header = string.Format(filePartHeader, temp.Prop, temp.Value);
                        var headerbytes = Encoding.UTF8.GetBytes(header);
                        memStream.Write(headerbytes, 0, headerbytes.Length);
                        var buffer = new byte[1024];
                        int bytesRead; // =0
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memStream.Write(buffer, 0, bytesRead);
                        }
                        string end = "\r\n";
                        headerbytes = Encoding.UTF8.GetBytes(end);
                        memStream.Write(headerbytes,0,headerbytes.Length);
                        fileStream.Close();
                    }
                    else if (temp.Type == 0)
                    {
                        // 写入字符串的Key
                        var stringKeyHeader = "Content-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}\r\n";
                        var header = string.Format(stringKeyHeader, temp.Prop, temp.Value);
                        var headerbytes = Encoding.UTF8.GetBytes(header);
                        memStream.Write(headerbytes, 0, headerbytes.Length);
                    }
                    if (i != postParaList.Count - 1)
                        memStream.Write(beginBoundary, 0, beginBoundary.Length);
                    else
                        // 写入最后的结束边界符
                        memStream.Write(endBoundary, 0, endBoundary.Length);
                }
                webRequest.ContentLength = memStream.Length;
                var requestStream = webRequest.GetRequestStream();
                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
                using (HttpWebResponse res = (HttpWebResponse)webRequest.GetResponse())
                {

                    using (Stream resStream = res.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            responseContent += Encoding.UTF8.GetString(buffer, 0, read);
                        }
                    }
                    res.Close();
                }
                return responseContent;
            }
            catch (Exception e)
            {
            }
            return null;


        }

        public static string postMessage(string strUrl, string strPost, string method="Post")
        {
            try
            {                
                CookieContainer objCookieContainer = null;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Method = method;
                request.Accept = "*/*";
                request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                request.Headers.Add("Accept-Charset: GBK,utf-8;q=0.7,*;q=0.3");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;

                request.Referer = strUrl;//.Remove(strUrl.LastIndexOf("/"));
                Console.WriteLine(strUrl);
                if (objCookieContainer == null)
                    objCookieContainer = new CookieContainer();

                request.CookieContainer = objCookieContainer;
                //Console.WriteLine(objCookieContainer.ToString());
                if (!string.IsNullOrEmpty(strPost))
                {
                    byte[] byteData = Encoding.UTF8.GetBytes(strPost.ToString().TrimEnd('&'));
                    request.ContentLength = byteData.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteData, 0, byteData.Length);
                        reqStream.Close();
                    }
                }

                string strResponse = "";
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                {
                    objCookieContainer = request.CookieContainer;
                    //QueryRecordForm.LoginCookie = objCookieContainer.GetCookies(new Uri(strUrl));
                    res.Cookies = objCookieContainer.GetCookies(new Uri(strUrl));
                    foreach (Cookie c in res.Cookies)
                    {

                    }

                    using (Stream resStream = res.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            strResponse += Encoding.UTF8.GetString(buffer, 0, read);
                        }
                    }
                    res.Close();
                }
                return strResponse;
            }
            catch (Exception e)
            {
            }
            return null;
        }

        public static string sendMessageCookie(string strUrl, string strPost, CookieContainer cookieContainer)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
                if (cookieContainer != null)
                {
                    request.CookieContainer = cookieContainer;
                }
                request.Method = "Post";
                request.Accept = "*/*";
                request.Headers.Add("Accept-Language: zh-CN,zh;q=0.8");
                request.Headers.Add("Accept-Charset: GBK,utf-8;q=0.7,*;q=0.3");               
                request.Headers.Add("Cache-Control: max-age=0");
                request.Accept = "text/xml,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";               
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
                request.Timeout = 10000;
                request.Referer = strUrl;//.Remove(strUrl.LastIndexOf("/"));

                if (!string.IsNullOrEmpty(strPost))
                {
                    request.ContentType = "application/json; text/html; charset=UTF-8";

                    byte[] byteData = Encoding.UTF8.GetBytes(strPost.ToString().TrimEnd('&'));
                    request.ContentLength = byteData.Length;
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(byteData, 0, byteData.Length);
                        reqStream.Close();
                    }
                }

                string strResponse = "";
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                {
                    if (cookieContainer != null)
                    {
                        cookieContainer = request.CookieContainer;
                    }
                    using (Stream resStream = res.GetResponseStream())
                    {
                        byte[] buffer = new byte[1024];
                        int read;
                        while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            strResponse += Encoding.UTF8.GetString(buffer, 0, read);
                        }
                    }
                    res.Close();
                }
                return strResponse;
            }
            catch (Exception e)
            {                
                //Console.WriteLine(e.ToString());
            }
            return null;
        }
  
    }


}

