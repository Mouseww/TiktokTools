using HtmlAgilityPack;
using Knyaz.Optimus;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting.Jint;
using Knyaz.Optimus.TestingTools;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using System.Windows.Forms;
using TiktokTools.Model;
using TikTokTools.Model;
using TikTokTools.Util;

namespace TikTokTools
{
    public partial class Form1 : Form
    {
        public List<VideoInfo> Awemes = new List<VideoInfo>();

        public Form1()
        {
            InitializeComponent();
        }

        Task task;

        delegate void SetTextCallback(string text);

        delegate void BinData(List<VideoInfo> awemes);
        private string KuaiShouDID;

        public bool IsFilePath { get; set; }

        public void Logout(string msg)
        {
            if (this.LogBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Logout);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                this.LogBox.AppendText(msg + "\r\n");
            }
        }

        public void ChangeStatus(string msg)
        {
            if (this.LogBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeStatus);
                this.Invoke(d, new object[] { msg });
            }
            else
            {
                if (msg == "start")
                {
                    btn_Stop.Visible = true;
                    btn_Start.Visible = false;
                }
                else
                {
                    btn_Start.Visible = true;
                    btn_Stop.Visible = false;
                }
            }
        }

        public void BinTableData(List<VideoInfo> awemes)
        {
            if (this.table_Video.InvokeRequired)
            {
                BinData d = new BinData(BinTableData);
                this.Invoke(d, new object[] { awemes });
            }
            else
            {
                this.table_Video.DataSource = awemes;
            }
        }

        CancellationTokenSource cts;

        CancellationToken ct
        {
            get { return cts.Token; }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            LogBox.Clear();
            btn_Stop.Visible = true;
            btn_Start.Visible = false;
            var configEntity = GetConfigEntity(txt_url.Text);

            var tab = tab_model.SelectedTab.Name;

            cts = new CancellationTokenSource();

            var videoInfoList = GetAlarmConfirmModelsByDGVCheckbox();
            task = new Task(() =>
             {
                 if (tab == "tabPage2")
                 {
                     string WebVideoPath = configEntity.LocalPath + "\\WebVideo\\";
                     if (false == System.IO.Directory.Exists(WebVideoPath))
                     {
                         System.IO.Directory.CreateDirectory(WebVideoPath);
                     }
                     configEntity.SourcePath = "";

                     Logout("共找到" + videoInfoList.Count + "个视频");
                     foreach (var item in videoInfoList)
                     {
                         ct.ThrowIfCancellationRequested();

                         var filename = Regex.Replace(item.Desc.Trim(), "[\\-_*×――(^)|'$%~!@#$…&%￥—+=<>\r\n《》!！??？:：•`·、。，；,.;\"‘’“”-]", "");
                         filename = filename.Trim() == "" ? DateTime.Now.ToString("yyyyMMddHHmmssfff") : filename;
                         filename = configEntity.LocalPath + "\\WebVideo\\" + filename + ".mp4";
                         try
                         {
                             DownloadFile(item.DownLink, filename, configEntity);

                         }
                         catch
                         {
                             Logout(item + "解析失败");
                         }

                         configEntity.SourcePath += configEntity.SourcePath == "" ? filename : "," + filename;
                     }
                 }
                 if (check_cmd.Checked)
                 {
                     var log = new ConvertHelperCmd.Run(Logout);
                     var changestatus = new ConvertHelperCmd.Run(ChangeStatus);

                     try
                     {
                         var result = new ConvertHelperCmd().Convert(configEntity, log, ct, changestatus);
                     }
                     catch (Exception ex)
                     {
                         log("程序已停止:" + ex.Message);
                         changestatus("Stop");
                     }
                 }
                 else
                 {
                     var log = new ConvertHelper.Run(Logout);
                     var changestatus = new ConvertHelper.Run(ChangeStatus);

                     try
                     {
                         var result = new ConvertHelper().Convert(configEntity, log, ct, changestatus);
                     }
                     catch (Exception ex)
                     {
                         log("程序已停止:" + ex.Message);
                         changestatus("Stop");
                     }
                 }

             });

            task.Start();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            btn_Start.Visible = true;
            btn_Stop.Visible = false;
            cts.Cancel();
            clearFolder(GetConfigEntity());
        }

        private ConfigEntity GetConfigEntity(string url = "douyin")
        {
            var random = new Random();
            var configEntity = new ConfigEntity()
            {
                SourcePath = SourcePathText.Text.ToLower().Contains(".mp4") ? SourcePathText.Text : SourcePathText.Text + '\\',
                LocalPath = Environment.CurrentDirectory,
                Remove_StartTime = !string.IsNullOrWhiteSpace(Remove_Left.Text) ? Convert.ToDouble(Remove_Left.Text) : 0.1,
                Remove_EndTime = !string.IsNullOrWhiteSpace(Remove_Right.Text) ? Convert.ToDouble(Remove_Right.Text) : 0.1,
                Video_Mirroring = Video_Mirroring.Checked,
                CenterTime = !string.IsNullOrWhiteSpace(Video_Center.Text) ? Convert.ToDouble(Video_Center.Text) : 3.5,
                ExtendTime = !string.IsNullOrWhiteSpace(Video_Center_Extend.Text) ? Convert.ToDouble(Video_Center_Extend.Text) : 0.01,
                ThreadNumber_Single = !string.IsNullOrWhiteSpace(ThreadNumber_SingleBox.Text) ? Convert.ToInt32(ThreadNumber_SingleBox.Text) : 32,
                Gamma = !string.IsNullOrWhiteSpace(text_Gamma.Text) ? Convert.ToDouble(text_Gamma.Text) : 1,
                Saturation = !string.IsNullOrWhiteSpace(text_Saturation.Text) ? Convert.ToDouble(text_Saturation.Text) : 1,
                Brightness = !string.IsNullOrWhiteSpace(text_Brightness.Text) ? Convert.ToDouble(text_Brightness.Text) : 0,
                Contrast = !string.IsNullOrWhiteSpace(text_Contrast.Text) ? Convert.ToDouble(text_Contrast.Text) : 1,
                Crop = !string.IsNullOrWhiteSpace(cqTextBox.Text) ? Convert.ToDecimal(cqTextBox.Text) : 3,
                Repeat = check_Repeat.Checked,
                Filter = check_Filter.Checked,
                IsFilePath = IsFilePath,
                VideoSource = url.ToLower().IndexOf("tiktok") > 0 ? VideoSource.TikTok : url.ToLower().IndexOf("douyin") > 0 ? VideoSource.DouYin : VideoSource.KuaiShou,
                AutoCZ = checkBoxCZ.Checked
            };


            return configEntity;
        }

        private void SetContorlVal(ConfigEntity configEntity)
        {
            Remove_Left.Text = configEntity.Remove_StartTime.ToString();
            Remove_Right.Text = configEntity.Remove_EndTime.ToString();
            Video_Mirroring.Checked = configEntity.Video_Mirroring;
            Video_Center.Text = configEntity.CenterTime.ToString();
            Video_Center_Extend.Text = configEntity.ExtendTime.ToString();
            ThreadNumber_SingleBox.Text = configEntity.ThreadNumber_Single.ToString();
            text_Gamma.Text = configEntity.Gamma.ToString();
            text_Saturation.Text = configEntity.Saturation.ToString();
            text_Brightness.Text = configEntity.Brightness.ToString();
            text_Contrast.Text = configEntity.Contrast.ToString();
            check_Repeat.Checked = configEntity.Repeat;
            check_Filter.Checked = configEntity.Filter;
            cqTextBox.Text = configEntity.Crop.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            SetContorlVal(GetConfigEntity());
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            table_Video.DataSource = videoInfos;
            Logout("");
        }


        private void check_Filter_CheckedChanged(object sender, EventArgs e)
        {
            if (check_Filter.Checked)
            {
                text_Gamma.Enabled = true;
                text_Brightness.Enabled = true;
                text_Contrast.Enabled = true;
                text_Saturation.Enabled = true;
            }
            else
            {
                text_Gamma.Enabled = false;
                text_Brightness.Enabled = false;
                text_Contrast.Enabled = false;
                text_Saturation.Enabled = false;
            }
        }

        private void btn_Chosen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SourcePathText.Text = dialog.SelectedPath;
                IsFilePath = false;
            }
        }

        private void btn_ChosenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*.*)|*.*";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SourcePathText.Text = string.Join(",", fileDialog.FileNames);
                IsFilePath = true;
            }
        }

        private void btn_OpenEnd_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", Environment.CurrentDirectory + "\\Finish");
        }

        private void check_cmd_CheckedChanged(object sender, EventArgs e)
        {

            if (check_cmd.Checked)
            {
                ThreadNumber_SingleBox.Visible = false;
                label6.Visible = false;
                label7.Visible = false;
            }
            else
            {
                ThreadNumber_SingleBox.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
            }
        }

        private void GetVideo(string url, string filename, ConfigEntity configEntity)
        {
            Logout("正在无水印解析视频...");

            List<PostDateClass> postDateClassList = new List<PostDateClass>();
            switch (configEntity.VideoSource)
            {

                case VideoSource.TikTok:
                    var result = GetTiktokVideo(url);
                    DownloadFile(result["videourl"].ToString(), filename);
                    break;
                case VideoSource.DouYin:
                    var videoLink = GetDouYinVideo(url);
                    DownloadFile(videoLink, filename);
                    break;
                case VideoSource.KuaiShou:
                    DownloadFile(GetKuaiShouVideo(url), filename);
                    break;
            }


        }


        private Hashtable GetTiktokVideo(string url)
        {
            var postDateClassList = new List<PostDateClass>();
            postDateClassList.Add(new PostDateClass("url", url));
            var getResult1 = HttpClientHelper.postMessage("https://downloaderi.com/tiktok-service.php", postDateClassList, true, "Post");
            return JsonConvert.DeserializeObject<Hashtable>(getResult1);
        }


        private string GetDouYinVideo(string url)
        {
            var html_302 = url;
            try
            {
                var urlTemp = HttpHelper.HttpGet(url);
                string reg = @"<a[^>]*href=([""'])?(?<href>[^'""]+)\1[^>]*>";
                urlTemp = Regex.Match(urlTemp, reg, RegexOptions.IgnoreCase).Groups["href"].Value;
                html_302 = HttpHelper.HttpGet(urlTemp);

            }
            catch
            {
                HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest1.UserAgent = "Mozilla/5.0(Linux;Android6.0;Nexus5Build/MRA58N)AppleWebKit / 537.36(KHTML, likeGecko)Chrome / 75.0.3770.100MobileSafari / 537.36";
                myHttpWebRequest1.AllowAutoRedirect = true;
                HttpWebResponse myHttpWebResponse1 = (HttpWebResponse)myHttpWebRequest1.GetResponse();
                html_302 = myHttpWebResponse1.ResponseUri.AbsoluteUri;
            }

            string vid = "";
            try
            {
                vid = Regex.Matches(html_302, "/(.*?)/")[2].Value.Split('/')[1].Split('?')[0];
            }
            catch
            {
                vid = html_302.Split('/')[html_302.Split('/').Length - 1].Split('?')[0];
            }
            var dytk = Regex.Match(Regex.Match(html_302, "dytk(.*?)}").Value, "\\w+(?=\")").Value;
            var getResult1 = HttpHelper.HttpGet("https://www.iesdouyin.com/web/api/v2/aweme/iteminfo/?item_ids=" + vid + "&dytk=" + dytk);
            var data1 = JsonConvert.DeserializeObject<Hashtable>(getResult1);
            var item_list = JsonConvert.DeserializeObject<List<Hashtable>>(data1["item_list"].ToString());
            var video = JsonConvert.DeserializeObject<Hashtable>(item_list[0]["video"].ToString());
            var play = JsonConvert.DeserializeObject<Hashtable>(video["play_addr"].ToString());
            var url_list = JsonConvert.DeserializeObject<List<string>>(play["url_list"].ToString());
            var videopath1 = url_list[0].ToString().Replace("playwm", "play");
            return GetVideoUrl(videopath1);
        }

        private string GetKuaiShouVideo(string url)
        {
            WebUtils webUtils = new WebUtils();
            var getResult = webUtils.DoGet("http://tapi.douhe.cloud/douhe/api/video/get_video?url=" + url);
            var data = JsonConvert.DeserializeObject<Hashtable>(getResult);
            return JsonConvert.DeserializeObject<Hashtable>(data["data"].ToString())["finalUri"].ToString();
        }

        private void BuildVideo(string url, string filename)
        {
            Spider spider = new Spider(url);
            var videoPath = spider.GetAttr("//a[1]", "href");
            DownloadFile(videoPath, filename);
        }

        private string GetVideoUrl(string videopath)
        {
            try
            {
                var html_302 = HttpHelper.HttpGet(videopath);
                string reg = @"<a[^>]*href=([""'])?(?<href>[^'""]+)\1[^>]*>";
                var link = Regex.Match(html_302, reg, RegexOptions.IgnoreCase).Groups["href"].Value;
                if (link.Contains("http://v3-dy-o.zjcdn.com"))
                {
                    return GetVideoUrl(videopath);
                }
                return link;
            }
            catch
            {
                return GetVideoUrl(videopath);
            }
        }

        private IList<VideoInfo> GetVideoList(string url)
        {
            Logout("正在解析用户的视频列表...");
            ConfigEntity configEntity = GetConfigEntity(url);
            IList<VideoInfo> result = new List<VideoInfo>();

            result = GetData(url, configEntity);
            return result;
        }

        private IList<VideoInfo> GetData(string url, ConfigEntity configEntity)
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            switch (configEntity.VideoSource)
            {
                case VideoSource.DouYin:
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                    myHttpWebRequest.AllowAutoRedirect = true;
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    string rederictUrl = myHttpWebResponse.ResponseUri.AbsoluteUri;
                    var sec_uid = new Uri(rederictUrl).AbsolutePath.Replace("/user/", "");// Query.Split('&')[1].Split('=')[1];
                    HttpItem httpItem1 = new HttpItem();
                    httpItem1.UserAgent = "user-agent=" + GetUA();
                    httpItem1.URL = "https://www.iesdouyin.com/web/api/v2/aweme/post/?sec_uid=" + sec_uid + "&count=1000";
                    var awemes = GetHtml(httpItem1).AwemeList;
                    videoInfos = awemes.Select(x => new VideoInfo(x)).ToList();
                    Parallel.ForEach<VideoInfo>(videoInfos, (item) =>
                    {
                        item.DownLink = GetDouYinVideo("https://www.iesdouyin.com/share/video/" + item.AwemeId);
                    });
                    return videoInfos;
                case VideoSource.KuaiShou:

                    return GetKuaiShouList(url);
            }
            return null;
        }

        private IList<VideoInfo> GetKuaiShouList(string url, double pcursor = 1.642669459008E12)
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            try
            {
                string uid = new Uri(url).AbsolutePath.Replace("/profile/", "");
                var query = new
                {
                    operationName = "visionProfilePhotoList",
                    variables = new
                    {
                        userId = uid,
                        pcursor = pcursor.ToString() + ".E12",
                        page = "profile"
                    },
                    query = "fragment photoContent on PhotoEntity {  id  duration  caption  likeCount  viewCount  realLikeCount  coverUrl  photoUrl  photoH265Url  manifest  manifestH265  videoResource  coverUrls {    url    __typename  }  timestamp  expTag  animatedCoverUrl  distance  videoRatio  liked  stereoType  profileUserTopPhoto  musicBlocked  __typename}fragment feedContent on Feed {  type  author {    id    name    headerUrl    following    headerUrls {      url      __typename    }    __typename  }  photo {    ...photoContent    __typename  }  canAddComment  llsid  status  currentPcursor  __typename}query visionProfilePhotoList($pcursor: String, $userId: String, $page: String, $webPageArea: String) {  visionProfilePhotoList(pcursor: $pcursor, userId: $userId, page: $page, webPageArea: $webPageArea) {    result    llsid    webPageArea    feeds {      ...feedContent      __typename    }    hostName    pcursor    __typename  }}"
                };
                WebHeaderCollection webHeader = new WebHeaderCollection();
                //webHeader.Add("Cookie",getKuaiShouCookie());
                webHeader.Add("Cookie", $"kpf=PC_WEB; kpn=KUAISHOU_VISION; clientid=3; did={getKuaiShouDId()}; userId=1967762954; kuaishou.server.web_ph=4010f04af86c86e788d2df188c640f790cf2;  kuaishou.server.web_st=ChZrdWFpc2hvdS5zZXJ2ZXIud2ViLnN0EqABO8byAqRkHfOOyT5Z9bQ0BvlJPZ2k5eD9ZKQSSV_1IvPGt7OCw-0xM3Hkydj49GWnjmkxL-KzKJLaFzAjnQMNQk8Ri5eORfvLU1e1soL5yYkM1bhF55fcafVR0O9fqoTTqejNFP2aWpcnrrabQU9NGttng9b8_jBrZtNeIa298qOIJdWgw306DCA5dDnRjhqH2LEecsIPRpL0Oz6jLc-fChoSdWlbobCW6oJxuQLJTUr9oj_uIiBNijinOYNjwm5uML9zrEwVOyte87hT2h5I_Kb5ETV7HigFMAE;");
                string result = HttpHelper.HttpPost("https://www.kuaishou.com/graphql", JsonConvert.SerializeObject(query), "application/json", webHeader);
                KuaiShouListVO kuaiShouListVO = JsonConvert.DeserializeObject<KuaiShouListVO>(result);
                foreach (var feed in kuaiShouListVO.data.visionProfilePhotoList.feeds)
                {
                    VideoInfo video = new VideoInfo();
                    video.DiggCount = feed.photo.realLikeCount.ToString();
                    video.ViewCount = feed.photo.viewCount.ToString();
                    video.AwemeId = feed.photo.id;
                    video.Desc = feed.photo.caption;
                    video.DownLink = feed.photo.videoResource.h264.adaptationSet[0].representation[0].url;
                    videoInfos.Add(video);
                }
            }
            catch
            {
                KuaiShouDID = null;
                return GetKuaiShouList(url, pcursor);
            }

            return videoInfos;
            
            if (pcursor<= 1.634E12)
            {
                return videoInfos;
            }

            Thread.Sleep(10000);
            videoInfos.AddRange(GetKuaiShouList(url, pcursor - 0.002E12 ).ToList());
            return videoInfos.GroupBy(x=>x.AwemeId).Select(x=>x.FirstOrDefault()).ToList();
        }

        private string getKuaiShouDId()
        {
            if (KuaiShouDID != null)
            {
                return KuaiShouDID;
            }

            KuaiShouDID = Interaction.InputBox("请设置快手网站的DIV (可在cookie中找到)", "设置快手DID", "在这里输入", -1, -1);
            return KuaiShouDID;
        }

        private string getKuaiShouCookie(string url = "https://www.kuaishou.com/")
        {

            HttpClient httpClient = new HttpClient();
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.RequestUri = new Uri(url);
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
            httpRequestMessage.Headers.Add("Sec-Fetch-User", "1");
            httpRequestMessage.Headers.Add("Sec-Fetch-Mode", "navigate");
            httpRequestMessage.Headers.Add("Host", "www.kuaishou.com");
            var response = httpClient.SendAsync(httpRequestMessage).GetAwaiter().GetResult();
            var cookies = response.Headers.GetValues("Set-Cookie");
            return String.Join("; ", cookies.Select(x => x.Split(';')[0]).ToArray());
        }

        //private string GetData(ChromeDriver driver,string uid) {
        //    ct.ThrowIfCancellationRequested();
        //    Thread.Sleep(2000);
        //    if (driver.FindElementByTagName("body").Text.Length>3)
        //    {
        //        return driver.FindElementByTagName("body").Text;
        //    }

        //    driver.ExecuteScript("document.location.href=document.location.href");

        //        return GetData(driver, uid);
        //}

        /// <summary>
        /// 获取当前的时间戳
        /// </summary>
        /// <returns></returns>
        public static string Timestamp()
        {
            long ts = ConvertDateTimeToInt(DateTime.Now);
            return ts.ToString();
        }

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            //System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            //long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            long t = (time.Ticks - 621356256000000000) / 10000;
            return t;
        }

        private DouYinModel GetHtml(HttpItem httpItem)
        {
            HttpHelper httpHelper = new HttpHelper();
            httpItem.Cookie = null;
            var htmlobj = httpHelper.GetHtml(httpItem);
            var reslutStr = htmlobj.Html;
            var result = JsonConvert.DeserializeObject<DouYinModel>(reslutStr);
            if (result.AwemeList == null || result.AwemeList.Count == 0)
            {
                httpHelper = null;
                return GetHtml(httpItem);
            }
            return result;
        }


        ///<summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">下载文件地址</param>
        /// <param name="Filename">下载后另存为（全路径）</param>
        private bool DownloadFile(string URL, string filename, ConfigEntity configEntity = null)
        {
            try
            {
                if (URL.IndexOf(".m3u8") > -1)
                {
                    new ConvertHelperCmd(configEntity).runcmd($" -i \"{URL}\" -vcodec copy -acodec copy -absf aac_adtstoasc \"{filename}\"");
                    return true;
                }

                //SetDownloadFile(URL, filename);
                WebClientPro webClient = new WebClientPro();
                webClient.Headers.Add("Host", new Uri(URL).Host);
                webClient.Timeout = 5 * 60 * 1000;
                //webClient.Headers.Add("User-Agent" , GetUA());
                webClient.DownloadFile(URL, filename);
                return true;
            }
            catch (System.Exception e)
            {
                return DownloadFile(URL, filename);
            }
        }

        ///<summary>
        /// 下载，通过http-url去下载文件到本地
        /// </summary>
        /// <param name="URL">下载文件地址:HTTP/HTTPS</param>
        /// <param name="Filename">下载后另存为（全路径）</param>
        /// <returns>成功时："1"。失败时：返回错误信息</returns>
        public void SetDownloadFile(System.String str_url, System.String str_filename)
        {
            var client = new HttpClient();
            var byts = client.GetByteArrayAsync(str_url).GetAwaiter().GetResult();
            using (var stream = File.Create(str_filename))
            {
                foreach (var byt in byts)
                {
                    stream.WriteByte(byt);
                }
            }
        }

        private bool IsInt(string str)
        {
            foreach (var item in str)
            {
                Int32.Parse(item.ToString());
            }
            return true;
        }


        private void clearFolder(ConfigEntity configEntity)
        {
            var files = Directory.GetFiles(configEntity.LocalPath + @"\Audio\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files)
            {
                try
                {
                    File.Delete(item);
                }
                catch { }
            }
            var files2 = Directory.GetFiles(configEntity.LocalPath + @"\Video\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files2)
            {
                try
                {
                    File.Delete(item);
                }
                catch { }
            }
            var files3 = Directory.GetFiles(configEntity.LocalPath + @"\WebVideo\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files3)
            {
                try
                {
                    File.Delete(item);
                }
                catch { }
            }
        }

        private void check_more_CheckedChanged(object sender, EventArgs e)
        {
            if (check_more.Checked)
            {
                lab_address.Text = "用户主页：";
                table_Video.Enabled = true;
            }
            else
            {
                lab_address.Text = "视频地址：";
                table_Video.Enabled = false;
            }
        }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Read_Click(object sender, EventArgs e)
        {
            BinTableData(new List<VideoInfo>());
            task = new Task(() =>
             {
                 if (check_more.Checked)
                 {
                     //GetVideoListByWebBrowser(txt_url.Text);
                     var videoList = GetVideoList(txt_url.Text);
                     BinTableData(videoList.ToList());
                     Logout("视频列表解析成功...");
                 }
                 else
                 {
                     var configEntity = GetConfigEntity(txt_url.Text);
                     var videoInfo = new VideoInfo();
                     switch (configEntity.VideoSource)
                     {
                         case VideoSource.TikTok:
                             var result = GetTiktokVideo(txt_url.Text);
                             videoInfo.AwemeId = txt_url.Text;
                             videoInfo.Desc = result["text"].ToString();
                             videoInfo.CommentCount = result["commentCount"].ToString();
                             videoInfo.DiggCount = result["diggCount"].ToString();
                             videoInfo.ViewCount = result["playCount"].ToString();
                             videoInfo.ShareCount = result["shareCount"].ToString();
                             videoInfo.DownLink = "https://downloaderi.com/downloadvideo.php?url=" + result["videourl"].ToString();
                             Awemes.Add(videoInfo);
                             BinTableData(Awemes);
                             break;
                         case VideoSource.DouYin:
                             var url = HttpHelper.HttpGet(txt_url.Text);
                             string vid = Regex.Matches(url, "/(.*?)/")[2].Value.Split('/')[1];
                             string reg = @"<a[^>]*href=([""'])?(?<href>[^'""]+)\1[^>]*>";
                             url = Regex.Match(url, reg, RegexOptions.IgnoreCase).Groups["href"].Value;
                             var html_302 = HttpHelper.HttpGet(url);
                             var dytk = Regex.Match(Regex.Match(html_302, "dytk(.*?)}").Value, "\\w+(?=\")").Value;
                             var getResult = HttpHelper.HttpGet("https://www.iesdouyin.com/web/api/v2/aweme/iteminfo/?item_ids=" + vid + "&dytk=" + dytk);
                             var data1 = JsonConvert.DeserializeObject<Hashtable>(getResult);
                             var item_list = JsonConvert.DeserializeObject<List<Hashtable>>(data1["item_list"].ToString());
                             videoInfo.AwemeId = item_list[0]["aweme_id"].ToString();
                             videoInfo.Desc = item_list[0]["desc"].ToString();
                             var staticdata = JsonConvert.DeserializeObject<Hashtable>(item_list[0]["statistics"].ToString());
                             videoInfo.CommentCount = staticdata["comment_count"].ToString();
                             videoInfo.DiggCount = staticdata["digg_count"].ToString();
                             videoInfo.DownLink = GetDouYinVideo(url);
                             BinTableData(new List<VideoInfo>() { videoInfo });
                             break;
                         case VideoSource.KuaiShou:
                             var didv = Timestamp();
                             var cookies = string.Format("oing_setcoo=1; didv={0}; did=web_241ecf6e16c34b42a5cf1847b1914622; sid=91bd7e19aaa7a53b8fdb2ca2; clientid=3; client_key=65890b29; Hm_lvt_86a27b7db2c5c0ae37fee4a8a35033ee=1591762140; Hm_lpvt_86a27b7db2c5c0ae37fee4a8a35033ee=1591762180; userId=1967762954", didv);

                             HttpItem httpItem = new HttpItem();
                             httpItem.URL = txt_url.Text;
                             httpItem.Cookie = cookies;
                             httpItem.UserAgent = "user-agent=" + GetUA();
                             HttpHelper httpHelper = new HttpHelper();
                             var htmlResult = httpHelper.GetHtml(httpItem);
                             Spider spider = new Spider(htmlResult.Html, 1);
                             var jsonStr = spider.GetAttr("//*[@id=\"hide-pagedata\"]", "data-pagedata");
                             var json = JsonConvert.DeserializeObject<Hashtable>(jsonStr.Replace("&#34;", "\""));
                             json = JsonConvert.DeserializeObject<Hashtable>(json["video"].ToString());
                             //DownloadFile(json["srcNoMark"].ToString(), filename);
                             break;
                         default:
                             var stringResult = HttpHelper.HttpGet($"https://tenapi.cn/video/?url={txt_url.Text}");
                             var data = JsonConvert.DeserializeObject<Hashtable>(stringResult);
                             videoInfo.Desc = data["title"].ToString();
                             videoInfo.DownLink = data["url"].ToString();
                             break;

                     }

                 }
             });
            task.Start();


        }

        private void table_Video_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            if (table_Video.Columns[e.ColumnIndex].Name != "select") return;

            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)table_Video.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell.Value != null && (bool)cell.Value)
            {
                cell.Value = false;
            }
            else
            {
                cell.Value = true;
            }
        }

        private List<VideoInfo> GetAlarmConfirmModelsByDGVCheckbox()
        {
            List<VideoInfo> alarmModels = new List<VideoInfo>();
            foreach (DataGridViewRow row in table_Video.Rows)
            {
                if (row.Cells["select"].Value != null && (bool)row.Cells["select"].Value)
                {
                    VideoInfo model = row.DataBoundItem as VideoInfo;
                    if (model == null) continue;
                    alarmModels.Add(model);
                }
            }
            return alarmModels.Count == 0 ? null : alarmModels;
        }

        private void check_all_CheckedChanged(object sender, EventArgs e)
        {
            int count = table_Video.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)table_Video.Rows[i].Cells[0];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (flag == false && check_all.Checked)
                {
                    checkCell.Value = true;
                }
                else if (!check_all.Checked)
                {
                    checkCell.Value = false;
                }
            }
        }



        private string GetUA()
        {
            return string.Format("Mozilla/5.0 (iPhone; CPU iPhone OS {0}_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1", new Random().Next(10, 13));

        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            Awemes.Clear();
            table_Video.DataSource = new List<VideoInfo>();
        }

        private void table_Video_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

        }

    }
}
