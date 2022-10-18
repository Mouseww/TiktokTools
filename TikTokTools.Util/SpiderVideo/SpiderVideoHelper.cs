using Microsoft.Playwright;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TikTokTools.Util.Model;
using static System.Net.Mime.MediaTypeNames;

namespace TikTokTools.Util.SpiderVideo
{
    public class SpiderVideoHelper
    {

        public string GetVideo(string url,string filename=null)
        {
            VideoSource videoSource = url.ToLower().IndexOf("tiktok") > 0 ? VideoSource.TikTok : url.ToLower().IndexOf("douyin") > 0 ? VideoSource.DouYin : url.ToLower().IndexOf("kuaishou") > 0 ? VideoSource.KuaiShou : VideoSource.Unknown;
            filename = filename ?? Guid.NewGuid().ToString() + ".mp4";
            List<PostDateClass> postDateClassList = new List<PostDateClass>();
            switch (videoSource)
            {

                case VideoSource.TikTok:
                    var result = GetTiktokVideo(url);
                    return DownloadFile(result["videourl"].ToString(),filename);
                case VideoSource.DouYin:
                    var videoLink = GetDouYinVideo(url);
                    return DownloadFile(videoLink, filename);
                case VideoSource.KuaiShou:
                    return DownloadFile(GetKuaiShouVideo(url).GetAwaiter().GetResult().DownLink, filename);
            }

            return null;
        }

        public Hashtable GetTiktokVideo(string url)
        {
            var postDateClassList = new List<PostDateClass>();
            postDateClassList.Add(new PostDateClass("url", url));
            var getResult1 = HttpClientHelper.postMessage("https://downloaderi.com/tiktok-service.php", postDateClassList, true, "Post");
            return JsonConvert.DeserializeObject<Hashtable>(getResult1);
        }

        public string GetDouYinVideo(string url)
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

        public async Task<VideoInfo> GetKuaiShouVideo(string url)
        {
            return GetKuaiShouVideoV2(url);


            VideoInfo videoInfo = new VideoInfo();
            try
            {
                HttpWebRequest myHttpWebRequest1 = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest1.UserAgent = "Mozilla/5.0(Linux;Android6.0;Nexus5Build/MRA58N)AppleWebKit / 537.36(KHTML, likeGecko)Chrome / 75.0.3770.100MobileSafari / 537.36";
                myHttpWebRequest1.AllowAutoRedirect = true;
                WebResponse myHttpWebResponse1 = (WebResponse)myHttpWebRequest1.GetResponse();
                CookieContainer cookie = new CookieContainer();
                cookie.SetCookies(new Uri("https://www.kuaishou.com"), myHttpWebResponse1.Headers.Get("Set-Cookie"));
                string ht=HttpHelper.HttpGet(myHttpWebResponse1.ResponseUri.AbsoluteUri,isUA:false, CookieContainer: cookie);
                //var data = JsonConvert.DeserializeObject<Hashtable>(apiResult["message"].ToString());
                //videoInfo.DownLink = data["photoUrl"].ToString();
                //videoInfo.Desc = data["caption"].ToString();
                //videoInfo.ViewCount = data["duration"].ToString();
                //videoInfo.DiggCount = data["realLikeCount"].ToString();
                //videoInfo.AwemeId = data["id"].ToString();
            }
            catch
            {

            }

            return videoInfo;
        }

        public VideoInfo GetKuaiShouVideoV2(string url)
        {
            VideoInfo videoInfo = new VideoInfo();
            string response = HttpHelper.HttpGet("http://douyin.fhcollege.com/shuiyin/index.php?url=" + url);
            var apiResult = JsonConvert.DeserializeObject<Hashtable>(response);
            var data = JsonConvert.DeserializeObject<Hashtable>(apiResult["message"].ToString());
            videoInfo.DownLink = data["photoUrl"].ToString();
            videoInfo.Desc = data["caption"].ToString();
            videoInfo.ViewCount = data["duration"].ToString();
            videoInfo.DiggCount = data["realLikeCount"].ToString();
            videoInfo.AwemeId = data["id"].ToString();
            return videoInfo;
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


        public IList<VideoInfo> GetData(string url, ConfigEntity configEntity)
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

                    return getKuaiShouListV2(url).GetAwaiter().GetResult();
            }
            return null;
        }
        private async Task<List<VideoInfo>> getKuaiShouListV2(string url, double pcursor = 1.661412644963E12, IPage page = null)
        {
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            string uid = new Uri(url).AbsolutePath.Replace("/profile/", "");
            if (page == null)
            {
                page = await GetBrowserPage(url);
            }

            var query = new
            {
                operationName = "visionProfilePhotoList",
                variables = new
                {
                    userId = uid,
                    pcursor = pcursor / (1E12) + "E12",

                    count = 999
                },
                query = "fragment photoContent on PhotoEntity {  id  duration  caption  likeCount  viewCount  realLikeCount  coverUrl  photoUrl  photoH265Url  manifest  manifestH265  videoResource  coverUrls {    url    __typename  }  timestamp  expTag  animatedCoverUrl  distance  videoRatio  liked  stereoType  profileUserTopPhoto  musicBlocked  __typename}fragment feedContent on Feed {  type  author {    id    name    headerUrl    following    headerUrls {      url      __typename    }    __typename  }  photo {    ...photoContent    __typename  }  canAddComment  llsid  status  currentPcursor  __typename}query visionProfilePhotoList($pcursor: String, $userId: String, $page: String, $webPageArea: String) {  visionProfilePhotoList(pcursor: $pcursor, userId: $userId, page: $page, webPageArea: $webPageArea) {    result    llsid    webPageArea    feeds {      ...feedContent      __typename    }    hostName    pcursor    __typename  }}"
            };
            try
            {
                KuaiShouListVO kuaiShouListVO = JsonConvert.DeserializeObject<KuaiShouListVO>(await (await page.Context.APIRequest.PostAsync("https://www.kuaishou.com/graphql", new APIRequestContextOptions() { DataObject = query })).TextAsync());
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
                await page.ReloadAsync();
                await page.WaitForSelectorAsync(".main");
                videoInfos.AddRange(await getKuaiShouListV2(url, pcursor));
            }

            if (pcursor <= 1.641412644963E12 )
            {
                return videoInfos;
            }

            Thread.Sleep(2000);
            videoInfos.AddRange(await getKuaiShouListV2(url, pcursor - 0.003000000000E12));
            return videoInfos.GroupBy(x => x.Desc).Select(x => x.FirstOrDefault()).OrderByDescending(x => Int32.Parse(x.ViewCount)).ToList();
        }

        private async Task<IPage> GetBrowserPage(string url, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions() { Headless = true });
            var page = await browser.NewPageAsync();
            if (headers != null)
            {
                page.SetExtraHTTPHeadersAsync(headers);
            }

            await page.GotoAsync(url);
            //await page.WaitForSelectorAsync(".main-content");
            return page;
        }


        private string GetUA()
        {
            return string.Format("Mozilla/5.0 (iPhone; CPU iPhone OS {0}_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1", new Random().Next(10, 13));

        }

        ///<summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">下载文件地址</param>
        /// <param name="Filename">下载后另存为（全路径）</param>
        public string DownloadFile(string URL, string filename, ConfigEntity configEntity = null)
        {
            try
            {
                if (URL.IndexOf(".m3u8") > -1)
                {
                    new ConvertHelperCmd(configEntity).runcmd($" -i \"{URL}\" -vcodec copy -acodec copy -absf aac_adtstoasc \"{filename}\"");
                    return filename;
                }

                WebClientPro webClient = new WebClientPro();
                webClient.Headers.Add("Host", new Uri(URL).Host);
                webClient.Timeout = 5 * 60 * 1000;
                webClient.DownloadFile(URL, filename);
                return filename;
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
    }
}
