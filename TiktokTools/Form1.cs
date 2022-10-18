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
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using TiktokTools.Model;
using TikTokTools.Util;
using TikTokTools.Util.Model;
using TikTokTools.Util.SpiderVideo;
using static System.Windows.Forms.LinkLabel;

namespace TikTokTools
{
    public partial class Form1 : Form
    {
        public List<VideoInfo> Awemes = new List<VideoInfo>();

        public SpiderVideoHelper videoHelper = new SpiderVideoHelper();

        private Task task;

        delegate void SetTextCallback(string text, string userId = null);

        delegate void BinData(List<VideoInfo> awemes);

        private CancellationTokenSource cts;

        private CancellationToken ct
        {
            get { return cts.Token; }
        }

        public bool IsFilePath { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        #region Form Event
        /// <summary>
        /// Start Button 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                             videoHelper.DownloadFile(item.DownLink, filename, configEntity);

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
                         var result = new ConvertHelperCmd().ConvertAsync(configEntity, log, ct, changestatus).GetAwaiter().GetResult();
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

        /// <summary>
        /// Stop 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            btn_Start.Visible = true;
            btn_Stop.Visible = false;
            cts.Cancel();
            clearFolder(GetConfigEntity());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetContorlVal(GetConfigEntity());
            //adjustType.ValueMember = "value";
            List<VideoInfo> videoInfos = new List<VideoInfo>();
            table_Video.DataSource = videoInfos;
            Logout("");
        }

        /// <summary>
        /// 文件夹路径选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 文件路径选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_OpenEnd_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("Explorer.exe", Environment.CurrentDirectory + "\\Finish");
        }

        /// <summary>
        /// 滤镜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// CMD / Library 模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                             var result = videoHelper.GetTiktokVideo(txt_url.Text);
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
                             videoInfo.DownLink = videoHelper.GetDouYinVideo(url);
                             BinTableData(new List<VideoInfo>() { videoInfo });
                             break;
                         case VideoSource.KuaiShou:
                             videoInfo = videoHelper.GetKuaiShouVideo(txt_url.Text).GetAwaiter().GetResult();
                             break;
                         default:
                             var stringResult = HttpHelper.HttpGet($"https://tenapi.cn/video/?url={txt_url.Text}");
                             var data = JsonConvert.DeserializeObject<Hashtable>(stringResult);
                             videoInfo.Desc = data["title"].ToString();
                             videoInfo.DownLink = data["url"].ToString();
                             break;

                     }
                     BinTableData(new List<VideoInfo>() { videoInfo });
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

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            Awemes.Clear();
            table_Video.DataSource = new List<VideoInfo>();
        }

        private void table_Video_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

        }
        #endregion

        #region Business Logic
        /// <summary>
        /// 绑定视频列表数据
        /// </summary>
        /// <param name="awemes"></param>
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

        /// <summary>
        /// 输出信息到LogBox
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        public void Logout(string msg, string userId = null)
        {
            if (this.LogBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Logout);
                this.Invoke(d, new object[] { msg, null });
            }
            else
            {
                this.LogBox.AppendText(msg + "\r\n");
            }
        }

        /// <summary>
        /// 控制Start Button 的文本变化
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userId"></param>
        public void ChangeStatus(string msg, string userId = null)
        {
            if (this.LogBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(ChangeStatus);
                this.Invoke(d, new object[] { msg, null });
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

        /// <summary>
        /// 从页面生成config
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
                VideoSource = url.ToLower().IndexOf("tiktok") > 0 ? VideoSource.TikTok : url.ToLower().IndexOf("douyin") > 0 ? VideoSource.DouYin : url.ToLower().IndexOf("kuaishou") > 0 ? VideoSource.KuaiShou : VideoSource.Unknown,
                AutoCZ = checkBoxCZ.Checked,
                CropType = !string.IsNullOrWhiteSpace(CropType.Text) ? Int32.Parse(CropType.Text) : 0,
            };


            return configEntity;
        }

        /// <summary>
        /// 根据Config设置页面控件的值
        /// </summary>
        /// <param name="configEntity"></param>
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
            CropType.Text = configEntity.CropType.ToString();
        }

        [Obsolete]
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

        /// <summary>
        /// 获取视频列表
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IList<VideoInfo> GetVideoList(string url)
        {
            Logout("正在解析用户的视频列表...");
            ConfigEntity configEntity = GetConfigEntity(url);
            IList<VideoInfo> result = new List<VideoInfo>();

            result = videoHelper.GetData(url, configEntity);
            return result;
        }

        /// <summary>
        /// 清理文件夹
        /// </summary>
        /// <param name="configEntity"></param>
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
        #endregion
    }
}
