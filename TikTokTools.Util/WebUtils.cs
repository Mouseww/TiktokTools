using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace TikTokTools.Util
{
    /// <summary>
    /// 网络工具类。
    /// </summary>
    public sealed class WebUtils
    {
        private int _timeout = 600000;
        private int _readWriteTimeout = 600000;
        private bool _ignoreSSLCheck = true;
        private bool _disableWebProxy = false;

        /// <summary>
        /// 等待请求开始返回的超时时间
        /// </summary>
        public int Timeout
        {
            get { return this._timeout; }
            set { this._timeout = value; }
        }

        /// <summary>
        /// 等待读取数据完成的超时时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return this._readWriteTimeout; }
            set { this._readWriteTimeout = value; }
        }

        /// <summary>
        /// 是否忽略SSL检查
        /// </summary>
        public bool IgnoreSSLCheck
        {
            get { return this._ignoreSSLCheck; }
            set { this._ignoreSSLCheck = value; }
        }

        /// <summary>
        /// 是否禁用本地代理
        /// </summary>
        public bool DisableWebProxy
        {
            get { return this._disableWebProxy; }
            set { this._disableWebProxy = value; }
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> textParams)
        {
            return DoPost(url, textParams, null);
        }

        /// <summary>
        /// 执行HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <param name="headerParams">请求头部参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> textParams, IDictionary<string, string> headerParams)
        {
            HttpWebRequest req = GetWebRequest(url, "POST", headerParams);
            req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            byte[] postData = Encoding.UTF8.GetBytes(BuildQuery(textParams));
            System.IO.Stream reqStream = req.GetRequestStream();
            reqStream.Write(postData, 0, postData.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = GetResponseEncoding(rsp);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url)
        {
            var req=WebRequest.CreateDefault(new Uri(url));
            req.Timeout = this._timeout;
            req.Method = "Get";
            var rsp = (HttpWebResponse)req.GetResponse();
            var end = GetResponseEncoding(rsp);
            string uu = GetResponseAsString(rsp, end);
            return uu;
        }

        /// <summary>
        /// 执行HTTP GET请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <param name="headerParams">请求头部参数</param>
        /// <returns>HTTP响应</returns>
        public string DoGet(string url, IDictionary<string, string> textParams, IDictionary<string, string> headerParams)
        {
            if (textParams != null && textParams.Count > 0)
            {
                url = BuildRequestUrl(url, textParams);
            }

            HttpWebRequest req = GetWebRequest(url, "GET", headerParams);
           // req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = GetResponseEncoding(rsp);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行带文件上传的HTTP POST请求。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="textParams">请求文本参数</param>
        /// <param name="fileParams">请求文件参数</param>
        /// <param name="headerParams">请求头部参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, IDictionary<string, string> textParams, IDictionary<string, FileItem> fileParams, IDictionary<string, string> headerParams)
        {
            // 如果没有文件参数，则走普通POST请求
            if (fileParams == null || fileParams.Count == 0)
            {
                return DoPost(url, textParams, headerParams);
            }

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线

            HttpWebRequest req = GetWebRequest(url, "POST", headerParams);
            req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

            System.IO.Stream reqStream = req.GetRequestStream();
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            // 组装文本请求参数
            string textTemplate = "Content-Disposition:form-data;name=\"{0}\"\r\nContent-Type:text/plain\r\n\r\n{1}";
            foreach (KeyValuePair<string, string> kv in textParams)
            {
                string textEntry = string.Format(textTemplate, kv.Key, kv.Value);
                byte[] itemBytes = Encoding.UTF8.GetBytes(textEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);
            }

            // 组装文件请求参数
            string fileTemplate = "Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            foreach (KeyValuePair<string, FileItem> kv in fileParams)
            {
                string key = kv.Key;
                FileItem fileItem = kv.Value;
                if (!fileItem.IsValid())
                {
                    throw new ArgumentException("FileItem is invalid");
                }
                string fileEntry = string.Format(fileTemplate, key, fileItem.GetFileName(), fileItem.GetMimeType());
                byte[] itemBytes = Encoding.UTF8.GetBytes(fileEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);
                fileItem.Write(reqStream);
            }

            reqStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = GetResponseEncoding(rsp);
            return GetResponseAsString(rsp, encoding);
        }

        /// <summary>
        /// 执行带body体的POST请求。
        /// </summary>
        /// <param name="url">请求地址，含URL参数</param>
        /// <param name="body">请求body体字节流</param>
        /// <param name="contentType">body内容类型</param>
        /// <param name="headerParams">请求头部参数</param>
        /// <returns>HTTP响应</returns>
        public string DoPost(string url, byte[] body, string contentType, IDictionary<string, FileItem> fileParams, IDictionary<string, string> headerParams)
        {
            HttpWebRequest req = GetWebRequest(url, "POST", headerParams);
            req.ContentType = contentType;

            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线 
            req.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;

            System.IO.Stream reqStream = req.GetRequestStream();
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");



            // 组装文件请求参数
            string fileTemplate = "Content-Disposition:form-data;name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            foreach (KeyValuePair<string, FileItem> kv in fileParams)
            {
                string key = kv.Key;
                FileItem fileItem = kv.Value;
                if (!fileItem.IsValid())
                {
                    throw new ArgumentException("FileItem is invalid");
                }
                string fileEntry = string.Format(fileTemplate, key, fileItem.GetFileName(), fileItem.GetMimeType());
                byte[] itemBytes = Encoding.UTF8.GetBytes(fileEntry);
                reqStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                reqStream.Write(itemBytes, 0, itemBytes.Length);
                fileItem.Write(reqStream);
            }
            reqStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            reqStream.Close();

            HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
            Encoding encoding = GetResponseEncoding(rsp);
            return GetResponseAsString(rsp, encoding);


        }

        public HttpWebRequest GetWebRequest(string url, string method, IDictionary<string, string> headerParams)
        {
            HttpWebRequest req = null;
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                if (this._ignoreSSLCheck)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(TrustAllValidationCallback);
                }
                req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                req = (HttpWebRequest)WebRequest.Create(url);
            }

            if (this._disableWebProxy)
            {
                req.Proxy = null;
            }

            if (headerParams != null && headerParams.Count > 0)
            {
                foreach (string key in headerParams.Keys)
                {
                    req.Headers.Add(key, headerParams[key]);
                }
            }

            req.ServicePoint.Expect100Continue = false;
            req.Method = method;
            req.KeepAlive = true;
            req.UserAgent = "top-sdk-net";
            req.Accept = "text/xml,text/javascript";
            req.Timeout = this._timeout;
            req.ReadWriteTimeout = this._readWriteTimeout;

            return req;
        }

        /// <summary>
        /// 把响应流转换为文本。
        /// </summary>
        /// <param name="rsp">响应流对象</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>响应文本</returns>
        public string GetResponseAsString(HttpWebResponse rsp, Encoding encoding)
        {
            Stream stream = null;
            StreamReader reader = null;

            try
            {
                // 以字符流的方式读取HTTP响应
                stream = rsp.GetResponseStream();
                if (Constants.CONTENT_ENCODING_GZIP.Equals(rsp.ContentEncoding, StringComparison.OrdinalIgnoreCase))
                {
                    stream = new GZipStream(stream, CompressionMode.Decompress);
                }
                reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
            finally
            {
                // 释放资源
                if (reader != null) reader.Close();
                if (stream != null) stream.Close();
                if (rsp != null) rsp.Close();
            }
        }

        /// <summary>
        /// 组装含参数的请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="parameters">请求参数映射</param>
        /// <returns>带参数的请求URL</returns>
        public static string BuildRequestUrl(string url, IDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                return BuildRequestUrl(url, BuildQuery(parameters));
            }
            return url;
        }

        /// <summary>
        /// 组装含参数的请求URL。
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="queries">一个或多个经过URL编码后的请求参数串</param>
        /// <returns>带参数的请求URL</returns>
        public static string BuildRequestUrl(string url, params string[] queries)
        {
            if (queries == null || queries.Length == 0)
            {
                return url;
            }

            StringBuilder newUrl = new StringBuilder(url);
            bool hasQuery = url.Contains("?");
            bool hasPrepend = url.EndsWith("?") || url.EndsWith("&");

            foreach (string query in queries)
            {
                if (!string.IsNullOrEmpty(query))
                {
                    if (!hasPrepend)
                    {
                        if (hasQuery)
                        {
                            newUrl.Append("&");
                        }
                        else
                        {
                            newUrl.Append("?");
                            hasQuery = true;
                        }
                    }
                    newUrl.Append(query);
                    hasPrepend = false;
                }
            }
            return newUrl.ToString();
        }

        /// <summary>
        /// 组装普通文本请求参数。
        /// </summary>
        /// <param name="parameters">Key-Value形式请求参数字典</param>
        /// <returns>URL编码后的请求数据</returns>
        public static string BuildQuery(IDictionary<string, string> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return null;
            }

            StringBuilder query = new StringBuilder();
            bool hasParam = false;

            foreach (KeyValuePair<string, string> kv in parameters)
            {
                string name = kv.Key;
                string value = kv.Value;
                // 忽略参数名或参数值为空的参数
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                {
                    if (hasParam)
                    {
                        query.Append("&");
                    }

                    query.Append(name);
                    query.Append("=");
                    // query.Append(HttpUtility.UrlEncode(value, Encoding.UTF8));
                    hasParam = true;
                }
            }

            return query.ToString();
        }

        public Encoding GetResponseEncoding(HttpWebResponse rsp)
        {
            string charset = rsp.CharacterSet;
            if (string.IsNullOrEmpty(charset))
            {
                charset = Constants.CHARSET_UTF8;
            }
            return Encoding.GetEncoding(charset);
        }

        private static bool TrustAllValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; // 忽略SSL证书检查
        }
    }

    public class Constants
    {
        public const string CONTENT_ENCODING_GZIP = "gzip";
        public const string CHARSET_UTF8 = "utf-8";
        public const string CTYPE_DEFAULT = "application/octet-stream";
        public const int READ_BUFFER_SIZE = 1024 * 4;
    }

    /// <summary>
    /// 文件元数据。
    /// 可以使用以下几种构造方法：
    /// 本地路径：new FileItem("C:/temp.jpg");
    /// 本地文件：new FileItem(new FileInfo("C:/temp.jpg"));
    /// 字节数组：new FileItem("abc.jpg", bytes);
    /// 输入流：new FileItem("abc.jpg", stream);
    /// </summary>
    public class FileItem
    {
        private Contract contract;

        /// <summary>
        /// 基于本地文件的构造器。
        /// </summary>
        /// <param name="fileInfo">本地文件</param>
        public FileItem(FileInfo fileInfo)
        {
            this.contract = new LocalContract(fileInfo);
        }

        /// <summary>
        /// 基于本地文件全路径的构造器。
        /// </summary>
        /// <param name="filePath">本地文件全路径</param>
        public FileItem(string filePath)
            : this(new FileInfo(filePath))
        {
        }

        /// <summary>
        /// 基于文件名和字节数组的构造器。
        /// </summary>
        /// <param name="fileName">文件名称（服务端持久化字节数组到磁盘时的文件名）</param>
        /// <param name="content">文件字节数组</param>
        public FileItem(string fileName, byte[] content)
            : this(fileName, content, null)
        {
        }

        /// <summary>
        /// 基于文件名、字节数组和媒体类型的构造器。
        /// </summary>
        /// <param name="fileName">文件名（服务端持久化字节数组到磁盘时的文件名）</param>
        /// <param name="content">文件字字节数组</param>
        /// <param name="mimeType">媒体类型</param>
        public FileItem(string fileName, byte[] content, string mimeType)
        {
            this.contract = new ByteArrayContract(fileName, content, mimeType);
        }

        /// <summary>
        /// 基于文件名和输入流的构造器。
        /// </summary>
        /// <param name="fileName">文件名称（服务端持久化输入流到磁盘时的文件名）</param>
        /// <param name="content">文件输入流</param>
        public FileItem(string fileName, Stream stream)
            : this(fileName, stream, null)
        {
        }

        /// <summary>
        /// 基于文件名、输入流和媒体类型的构造器。
        /// </summary>
        /// <param name="fileName">文件名（服务端持久化输入流到磁盘时的文件名）</param>
        /// <param name="content">文件输入流</param>
        /// <param name="mimeType">媒体类型</param>
        public FileItem(string fileName, Stream stream, string mimeType)
        {
            this.contract = new StreamContract(fileName, stream, mimeType);
        }

        public bool IsValid()
        {
            return this.contract.IsValid();
        }

        public long GetFileLength()
        {
            return this.contract.GetFileLength();
        }

        public string GetFileName()
        {
            return this.contract.GetFileName();
        }

        public string GetMimeType()
        {
            return this.contract.GetMimeType();
        }

        public void Write(Stream output)
        {
            this.contract.Write(output);
        }
    }

    internal interface Contract
    {
        bool IsValid();
        string GetFileName();
        string GetMimeType();
        long GetFileLength();
        void Write(Stream output);
    }

    internal class LocalContract : Contract
    {
        private FileInfo fileInfo;

        public LocalContract(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        public long GetFileLength()
        {
            return this.fileInfo.Length;
        }

        public string GetFileName()
        {
            return this.fileInfo.Name;
        }

        public string GetMimeType()
        {
            return Constants.CTYPE_DEFAULT;
        }

        public bool IsValid()
        {
            return this.fileInfo != null && this.fileInfo.Exists;
        }

        public void Write(Stream output)
        {
            using (BufferedStream bfs = new BufferedStream(this.fileInfo.OpenRead()))
            {
                int n = 0;
                byte[] buffer = new byte[Constants.READ_BUFFER_SIZE];
                while ((n = bfs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, n);
                }
            }
        }
    }

    internal class ByteArrayContract : Contract
    {
        private string fileName;
        private byte[] content;
        private string mimeType;

        public ByteArrayContract(string fileName, byte[] content, string mimeType)
        {
            this.fileName = fileName;
            this.content = content;
            this.mimeType = mimeType;
        }

        public bool IsValid()
        {
            return this.content != null && this.fileName != null;
        }

        public long GetFileLength()
        {
            return this.content.Length;
        }

        public string GetFileName()
        {
            return this.fileName;
        }

        public string GetMimeType()
        {
            if (string.IsNullOrEmpty(this.mimeType))
            {
                return Constants.CTYPE_DEFAULT;
            }
            else
            {
                return this.mimeType;
            }
        }

        public void Write(Stream output)
        {
            output.Write(this.content, 0, this.content.Length);
        }
    }

    internal class StreamContract : Contract
    {
        private string fileName;
        private Stream stream;
        private string mimeType;

        public StreamContract(string fileName, Stream stream, string mimeType)
        {
            this.fileName = fileName;
            this.stream = stream;
            this.mimeType = mimeType;
        }

        public long GetFileLength()
        {
            return 0L;
        }

        public string GetFileName()
        {
            return this.fileName;
        }

        public string GetMimeType()
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                return Constants.CTYPE_DEFAULT;
            }
            else
            {
                return this.mimeType;
            }
        }

        public bool IsValid()
        {
            return this.stream != null && this.fileName != null;
        }

        public void Write(Stream output)
        {
            using (this.stream)
            {
                int n = 0;
                byte[] buffer = new byte[Constants.READ_BUFFER_SIZE];
                while ((n = this.stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, n);
                }
            }
        }
    }
}
