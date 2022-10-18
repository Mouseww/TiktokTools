using MediaInfoDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TikTokTools.Util;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace TikTokTools.Util
{
    public class InputFileInfo
    {
        internal long bitRate;
        internal bool hasAudio;
        internal double duration;
        internal float frameRate;
        internal double width;
        internal double height;

        public InputFileInfo(MediaFile file)
        {
            var video = file.Video.FirstOrDefault();
            bitRate = long.Parse(video.BitRate);
            duration = double.Parse(video.Duration.ToString()) / 1000;
            frameRate = video.frameRate;
            width = video.Width;
            height = video.Height;
            hasAudio = file.Audio != null && file.Audio.Count > 0;
        }

        public InputFileInfo(IMediaInfo file)
        {
            var video = file.VideoStreams.FirstOrDefault();
            bitRate = video.Bitrate;
            duration = video.Duration.TotalSeconds;
            frameRate = float.Parse(video.Framerate.ToString());
            width = video.Width;
            height = video.Height;
            hasAudio = file.AudioStreams != null && file.AudioStreams.Count() > 0;
        }

        public IMediaInfo File { get; }
    }

    public class ConvertHelperCmd
    {
        public delegate void Run(string msg, string userId = null);
        Run log;
        ConfigEntity configEntity;
        CancellationToken ct;
        InputFileInfo inputFile;
        public ConvertHelperCmd()
        {
        }

        public ConvertHelperCmd(ConfigEntity _configEntity)
        {
            configEntity = _configEntity;
        }

        public async Task<List<string>> ConvertAsync(ConfigEntity configEntityTemp, Run logtemp, CancellationToken cttemp, Run changestatus)
        {
            FFmpeg.SetExecutablesPath(configEntityTemp.LocalPath);
            var result = new List<string>();
            log = logtemp;
            ct = cttemp;
            configEntity = configEntityTemp;
            CreatFolder();
            List<string> files = new List<string>();
            files = configEntity.SourcePath.ToLower().Contains(".mp4") ?
                configEntity.SourcePath.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList()
                : Directory.GetFiles(configEntity.SourcePath, ".", SearchOption.AllDirectories).ToList();
            log(string.Format("共有{0}个视频待处理", files.Count), configEntity.UserID);

            for (int i = 0; i < files.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    log(string.Format("当前正在处理第{0}条", i + 1), configEntity.UserID);
                    result.Add(ConvertVideo(files[i],log,configEntity,ct));
                    log($"第{i}条处理完成", configEntity.UserID);
                }
                catch (Exception ex)
                {
                    log("出现错误..." + ex.Message, configEntity.UserID);
                    log("正在清理临时文件...", configEntity.UserID);
                    clearFolder(configEntity);
                    ct.ThrowIfCancellationRequested();
                }
            }

            try
            {
                var files3 = Directory.GetFiles(configEntity.LocalPath + @"\TempSource\", ".", SearchOption.AllDirectories).ToList();
                foreach (var item in files3)
                {
                    File.Delete(item);
                }
            }
            catch
            {
                var files3 = Directory.GetFiles(configEntity.LocalPath + @"\WebVideo\", ".", SearchOption.AllDirectories).ToList();
                foreach (var item in files3)
                {
                    File.Delete(item);
                }
            }

            log(" 全部视频处理完成...", configEntity.UserID);
            changestatus("Stop");
            return result;
        }

        private string ConvertVideo(string filepath, Run log, ConfigEntity configEntity, CancellationToken ct)
        {
            Match match = Regex.Match(filepath, @".+\\(.+)");
            string filename = match.Groups[1].Value.Replace(".mp4", "");
            string inputpath = filepath;
            string audiopath = (configEntity.LocalPath + @"\Audio\" + filename + ".mp3").Replace("\\\\", "\\");
            string videopath = (configEntity.LocalPath + @"\Video\" + filename + ".mp4").Replace("\\\\", "\\");
            string output = (configEntity.LocalPath + @"\Video\" + filename + "_temp.mp4").Replace("\\\\", "\\");
            try
            {
                var file = new MediaInfoDotNet.MediaFile(filepath);
                inputFile = new InputFileInfo(file);
            }
            catch
            {
                var file = FFmpeg.GetMediaInfo(filepath).GetAwaiter().GetResult();
                inputFile = new InputFileInfo(file);
            }
            ct.ThrowIfCancellationRequested();
            if (inputFile.hasAudio)
            {
                log("正在抽离音频...", configEntity.UserID);

                BuildAudio(inputpath, audiopath);
                ct.ThrowIfCancellationRequested();
            }

            log("正在进行调整视频...", configEntity.UserID);
            inputpath = AdjustVideo(inputpath, videopath);
            ct.ThrowIfCancellationRequested();

            log("正在进行视频分段调速处理...", configEntity.UserID);
            AEVideo(inputpath, output);
            ct.ThrowIfCancellationRequested();

            if (inputFile.hasAudio)
            {

                inputpath = output.ToString();
                log("正在将音频加入新的视频中...", configEntity.UserID);
                output = output.Replace(".mp4", "_Audio.mp4");
                AddAudio(inputpath, audiopath, output);
            }

            copyToFinishFolder(output, output.Replace("\\Video\\", "\\Finish\\"));
            log("正在清理临时文件...", configEntity.UserID);
            clearFolder(configEntity);
            ct.ThrowIfCancellationRequested();
            return output.Replace("\\Video\\", "\\Finish\\");
        }

        private void copyToFinishFolder(string output, string v)
        {
            runcmd($" -i \"{output}\" -c:v copy \"{v}\"");
        }

        private void clearFolder(ConfigEntity configEntity)
        {
            var files = Directory.GetFiles(configEntity.LocalPath + @"\Audio\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files)
            {
                File.Delete(item);
            }
            var files2 = Directory.GetFiles(configEntity.LocalPath + @"\Video\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files2)
            {
                File.Delete(item);
            }

        }

        /// <summary>
        /// Split Audio and Video
        /// </summary>
        /// <param name="input"></param>
        /// <param name="audiopath"></param>
        /// <returns></returns>
        private void BuildAudio(string input, string audiopath)
        {
            runcmd($" -y -i \"{input}\" -f mp3 \"{audiopath}\"");
        }

        /// <summary>
        /// 特殊调整（滤镜、画中画）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="videopath"></param>
        /// <returns></returns>
        private string AdjustVideo(string input, string videopath)
        {
            var result = input.ToString();
            log("正在调整视频...");
            var outpath = configEntity.Repeat ? input.Replace(".mp4", "_Adjust.mp4") : videopath;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($" -hwaccel qsv  -noautorotate -i \"{input}\" -b:v {(inputFile.bitRate / 1000 / 3 * 5)}k ");
            if (configEntity.Filter)
            {
                stringBuilder.Append($" -vf eq=contrast={configEntity.Contrast}:brightness={configEntity.Brightness}:saturation={configEntity.Saturation}:gamma={configEntity.Gamma}");
            }

            if (configEntity.Video_Mirroring)
            {
                stringBuilder.Append($" -vf hflip ");//视频镜像
            }

            if (configEntity.Repeat)
            {
                //stringBuilder.Append($" -filter_complex  overlay ");//画中画
            }

            switch (configEntity.CropType)
            {
                case 2:
                    ///竖转横
                    stringBuilder.Append($" -lavfi \"[0:v]scale=ih*16/9:-1,boxblur=luma_radius=min(h\\,w)/20:luma_power=1:chroma_radius=min(cw\\,ch)/20:chroma_power=1[bg];[bg][0:v]overlay=(W-w)/2:(H-h)/2,crop=h=iw*9/16\" ");
                    break;
                case 1:
                    ///横转竖
                    stringBuilder.Append($" -lavfi \"[0:v]scale=256/81*iw:256/81*ih,boxblur=luma_radius=min(h\\,w)/40:luma_power=3:chroma_radius=min(cw\\,ch)/40:chroma_power=1[bg];[bg][0:v]overlay=(W-w)/2:(H-h)/2,setsar=1,crop=w=iw*81/256\"");
                    break;
                default:
                    //正常裁剪
                    stringBuilder.Append($" -vf crop=iw-{1.5 * 2}*iw/100:ih-{configEntity.Crop * 2}*ih/100:{1.5}*iw/100:{configEntity.Crop}*ih/100 -s {inputFile.width}*{inputFile.height} ");
                    break;
            }


            stringBuilder.Append($" -vcodec h264  \"{outpath}\" -y");
            runcmd(stringBuilder.ToString());
            result = outpath;
            log("视频调整完毕...");
            return result;
        }

        /// <summary>
        /// 增加音频到视频
        /// </summary>
        /// <param name="videopath"></param>
        /// <param name="audiopath"></param>
        /// <param name="finishpath"></param>
        /// <returns></returns>
        private void AddAudio(string videopath, string audiopath, string finishpath)
        {
            runcmd(string.Format("-hwaccel qsv -noautorotate -i \"{0}\" -itsoffset 00:00:00.1 -i \"{1}\" -vcodec copy -acodec copy \"{2}\" -y", videopath, audiopath, finishpath));
        }

        /// <summary>
        /// 拆分 微调整顺序
        /// </summary>
        /// <param name="inputpath"></param>
        /// <param name="outputpath"></param>
        /// <returns></returns>
        private void AEVideo(string inputpath, string outputpath)
        {
            string guid = Guid.NewGuid().ToString();
            var zs = 1 / inputFile.frameRate;
            var txt = File.CreateText(configEntity.LocalPath + $"\\FileList{guid}.txt");
            double baseChangeSeconds = inputFile.duration / 10;
            for (double i = 1; i <= inputFile.duration; i += baseChangeSeconds)
            {
                var tempOut = outputpath.Replace(".mp4", "_temp" + i + ".mp4");
                double speed = new Random().Next(2, 6) * 0.25;
                if (i == 1)
                {
                    runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.Remove_StartTime).ToString(), TimeSpan.FromSeconds(baseChangeSeconds).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", speed), ""));
                }
                else if (i + baseChangeSeconds > inputFile.duration)
                {
                    runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(i - baseChangeSeconds + zs).ToString(), TimeSpan.FromSeconds(inputFile.duration - (i - baseChangeSeconds)).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", speed), ""));
                }
                else
                {
                    runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(i - baseChangeSeconds + zs).ToString(), TimeSpan.FromSeconds(baseChangeSeconds).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", speed), "-vf fade=in:0:1"));
                }
                txt.WriteLine("file '" + tempOut + "'");
            }

            txt.Close();
            runcmd(string.Format("-hwaccel qsv -noautorotate -f concat -safe 0 -i \"{0}\"  -b:v " + (inputFile.bitRate / 1000 / 3 * 5) + "k -bufsize " + (inputFile.bitRate / 1000 / 3 * 5) + "k  -c copy -vcodec copy  \"{1}\" -y", configEntity.LocalPath + $"\\FileList{guid}.txt", outputpath));
            File.Delete(configEntity.LocalPath + $"\\FileList{guid}.txt");
        }

        /// <summary>
        /// 使用ffmpeg.exe 控制台命令操作视频
        /// </summary>
        /// <param name="code"></param>
        public void runcmd(string code)
        {
            string str = code;
            string[] errorArray = new string[] { "invalid", "failed", "can not", "exception" };
            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo.FileName = (configEntity.LocalPath + @"\ffmpeg.exe").Replace("\\\\", "\\");
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.StartInfo.Arguments = str + " -threads 16 -preset ultrafast ";
                p.Start();//启动程序

                //向cmd窗口发送输入信息
                //p.StandardInput.WriteLine("cd " + path);

                p.StandardInput.WriteLine("&exit");

                p.StandardInput.AutoFlush = true;
                var output = p.StandardError.ReadToEnd();
                if (errorArray.Any(x=>output.ToLower().IndexOf(x)>-1))
                {
                    throw new Exception(str + "\r\n" + output);
                }

                p.WaitForExit();
                p.Close();
            }

        }

        /// <summary>
        /// 拆分出一个Video
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string SplitVideo(params string[] param)
        {
            var rui = new Random();
            return string.Format(
                "-hwaccel qsv -noautorotate -ss {1}  -i \"{0}\" -to {2}   -b:v " + (inputFile.bitRate / 1000 / 3 * 5) + "k -strict experimental -an  {4} {5} " +
                "-vf unsharp=luma_msize_x=7:luma_msize_y=7:luma_amount=" + rui.Next(1, 15) * 0.1 + " " +//轻度锐化 
                "-vcodec h264  \"{3}\"", param);
        }

        private void CreatFolder()
        {
            //获取当前文件夹路径
            string currPath = configEntity.LocalPath;

            string videoPath = currPath + "/Video/";
            if (false == System.IO.Directory.Exists(videoPath))
            {
                System.IO.Directory.CreateDirectory(videoPath);
            }
            string audioPath = currPath + "/Audio/";
            if (false == System.IO.Directory.Exists(audioPath))
            {
                System.IO.Directory.CreateDirectory(audioPath);
            }

            string finishPath = currPath + "/Finish/";
            if (false == System.IO.Directory.Exists(finishPath))
            {
                System.IO.Directory.CreateDirectory(finishPath);
            }

            string sourcePath = currPath + "/Source/";
            if (false == System.IO.Directory.Exists(sourcePath))
            {
                System.IO.Directory.CreateDirectory(sourcePath);
            }
        }
    }
}
