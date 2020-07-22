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
    public class ConvertHelperCmd
    {
        public delegate void Run(string msg);
        Run log;
        ConfigEntity configEntity;
        CancellationToken ct;
        IMediaInfo inputFile;
        public ConvertHelperCmd() {
        }

        public List<string> Convert(ConfigEntity configEntityTemp, Run logtemp, CancellationToken cttemp, Run changestatus)
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
            log(string.Format("共有{0}个视频待处理", files.Count));
            
            for (int i = 0; i < files.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    log(string.Format("当前正在处理第{0}条", i + 1));
                    Match match = Regex.Match(files[i], @".+\\(.+)");
                    string filename = match.Groups[1].Value.Replace(".mp4","");
                    string inputpath = files[i];
                    string audiopath = (configEntity.LocalPath + @"\Audio\" + filename + ".mp3").Replace("\\\\", "\\");
                    string videopath = (configEntity.LocalPath + @"\Video\" + filename + ".mp4").Replace("\\\\", "\\");
                    string finishpath = (configEntity.LocalPath + @"\Finish\" + filename + ".mp4").Replace("\\\\", "\\");
                    string output = (configEntity.LocalPath + @"\Video\" + filename + "_temp.mp4").Replace("\\\\", "\\");
                    inputFile = FFmpeg.GetMediaInfo(inputpath).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    if (inputFile.AudioStreams != null && inputFile.AudioStreams.Count() > 0)
                    {
                        log("正在抽离音频...");

                        BuildAudio(inputpath, audiopath).GetAwaiter().GetResult();
                        ct.ThrowIfCancellationRequested();
                    }
                    
                    log("正在进行调整视频...");
                    inputpath=AdjustVideo(inputpath, videopath);
                    ct.ThrowIfCancellationRequested();
                    
                    log("正在进行视频帧处理...");
                    AEVideo(inputpath, output).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    
                    
                    if (inputFile.AudioStreams != null && inputFile.AudioStreams.Count() > 0)
                    {

                        inputpath = output.ToString();
                        log("正在将音频加入新的视频中...");
                        output = output.Replace(".mp4", "_Audio.mp4");
                        AddAudio(inputpath, audiopath, output).GetAwaiter().GetResult();
                    }
                    inputpath = output;
                    log("正在将进行视频信息处理...");
                    changeVideo(videopath, finishpath).GetAwaiter().GetResult();
                    log("视频信息处理完毕...");
                    log("正在清理临时文件...");
                    clearFolder(configEntity);
                    result.Add(finishpath);
                    ct.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    log("出现错误..." + ex.Message);
                    log("正在清理临时文件...");
                    clearFolder(configEntity);
                    ct.ThrowIfCancellationRequested();
                }
            }
            var files3 = Directory.GetFiles(configEntity.LocalPath + @"\WebVideo\", ".", SearchOption.AllDirectories).ToList();
            foreach (var item in files3)
            {
                File.Delete(item);
            }
            log(" 全部视频处理完成...");
            changestatus("Stop");
            return result;
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



        private async Task BuildAudio(string input, string audiopath)
        {
            var video = await FFmpeg.Conversions.FromSnippet.ExtractAudio(input, audiopath);
            await video.Start();
        }

      
        /// <summary>
        /// 特殊调整（滤镜、画中画）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="videopath"></param>
        /// <returns></returns>
        private string AdjustVideo(string input,string videopath)
        {
            var result = input.ToString();
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            if (configEntity.Filter || configEntity.Video_Mirroring)
            {
                log("正在调整视频...");
                var outpath = configEntity.Repeat? input.Replace(".mp4", "_Adjust.mp4"):videopath;
                
                runcmd(string.Format(" -hwaccel qsv  -noautorotate -i \"{0}\" -b:v {4}k  {1}  {2}  -vcodec h264_qsv  \"{3}\" -y",
                   input,
                   configEntity.Filter ? string.Format("-vf eq=contrast={0}:brightness={1}:saturation={2}:gamma={3}", configEntity.Contrast,
                   configEntity.Brightness,
                   configEntity.Saturation,
                   configEntity.Gamma) : "",
                   configEntity.Video_Mirroring ? "-vf hflip" : "", outpath, (oldvideo.Bitrate/1000 +20)
                   ));

                input = outpath;
                result = input.ToString();
                log("视频调整完毕...");
            }

            if (configEntity.Repeat)
            {
                log("正在进行画中画...");
                runcmd(string.Format(" -hwaccel qsv -noautorotate -i \"{0}\" -i \"{1}\" -b:v "+(oldvideo.Bitrate / 1000 + 20) +"k -filter_complex  overlay  -vcodec h264_qsv  \"{2}\" -y", input, input, videopath));
                //runcmd(string.Format("-i \"{0}\" -i \"{0}\" \"nullsrc=size=200x200 [base]; [0:v] setpts=PTS-STARTPTS,scale=200x200 [left]; [1:v] setpts=PTS-STARTPTS, scale=100x100 [right];[base][left] overlay=shortest=1 [tmp1]; [tmp1][right] overlay=shortest=1:x=0\" -c:v h264_qsv  {1} - y", input, videopath));
                result = videopath.ToString();
                log("视频画中画成功...");
            }
            return result;
        }

        /// <summary>
        /// 增加音频到视频
        /// </summary>
        /// <param name="videopath"></param>
        /// <param name="audiopath"></param>
        /// <param name="finishpath"></param>
        /// <returns></returns>
        private async Task AddAudio(string videopath, string audiopath, string finishpath)
        {
            runcmd(string.Format("-hwaccel qsv -noautorotate -i \"{0}\" -itsoffset 00:00:00.1 -i \"{1}\" -vcodec copy -acodec copy \"{2}\" -y", videopath, audiopath, finishpath));

            //var random = new Random();
            //var video = await FFmpeg.Conversions.FromSnippet.AddAudio(videopath, audiopath, finishpath);

            //video.SetVideoBitrate(oldvideo.Bitrate + configEntity.Video_BitrateChange);
            ////video.SetPixelFormat(PixelFormat.yuyv422);
            //video.SetSeek(TimeSpan.FromSeconds(random.Next(1, 10) * 0.01));
            //video.SetHashFormat(Hash.SHA512_256);
            //video.SetFrameRate(oldvideo.Framerate + configEntity.Video_FrameRateChange);
            //video.UseMultiThread(configEntity.ThreadNumber_Single);
            //await video.Start();
        }

        /// <summary>
        /// 最终调整
        /// </summary>
        /// <param name="inputpath"></param>
        /// <param name="outpath"></param>
        /// <returns></returns>
        private async Task changeVideo(string inputpath, string outpath)
        {
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            
            runcmd(string.Format("-hwaccel qsv -noautorotate -i \"{0}\"  -b:v " + (oldvideo.Bitrate / 1000 + 20) + "k   -r {1} " +
                "-vf crop=iw-2*iw/100:ih-2*ih/100:iw/100:ih/100 -s {3}*{4} " +//剪切+分辨率设置
                                                                              //"-vf noise=alls=20:allf=t+u " +//降噪
                                                                              //"-vf unsharp=luma_msize_x=7:luma_msize_y=7:luma_amount=1.3 " +//轻度锐化
                                                                              //"-vf hue=\"H=2*PI*t:s=sin(2*PI*t)+1\" " +//色彩变化
                                                                              //"-vf fade=in:0:90 " +//渐入
                                                                              //"\"{3}\" -y", inputpath, oldvideo.Bitrate / 1000 + configEntity.Video_BitrateChange, oldvideo.Framerate + configEntity.Video_FrameRateChange, outpath));
                "-vcodec h264_qsv  \"{2}\" -y", inputpath,60, outpath,oldvideo.Width, oldvideo.Height));
        }

        /// <summary>
        /// 拆分 微调整顺序
        /// </summary>
        /// <param name="inputpath"></param>
        /// <param name="outputpath"></param>
        /// <returns></returns>
        private async Task AEVideo(string inputpath, string outputpath)
        {
            string output1 = outputpath.Replace(".mp4", "_temp1.mp4");
            string output2 = outputpath.Replace(".mp4", "_temp2.mp4");
            string output3 = outputpath.Replace(".mp4", "_temp3.mp4");
            string output4 = outputpath.Replace(".mp4", "_temp4.mp4");
            string output5 = outputpath.Replace(".mp4", "_temp5.mp4");
            string output6 = outputpath.Replace(".mp4", "_temp6.mp4");
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            var zs = 1/oldvideo.Framerate ;
            var txt = File.CreateText(configEntity.LocalPath + @"\FileList.txt");
            if (configEntity.AutoCZ)
            {
                for (int i = 1; i <= oldvideo.Duration.TotalSeconds; i++)
                {
                    var tempOut = outputpath.Replace(".mp4", "_temp" + i + ".mp4");
                    if (i == 1)
                    {
                        runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.Remove_StartTime).ToString(), TimeSpan.FromSeconds(1).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), ""));
                    } else if (i+1> oldvideo.Duration.TotalSeconds) {
                        runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(i - 1 + zs).ToString(), TimeSpan.FromSeconds(inputFile.Duration.TotalSeconds - (i - 1)).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), ""));
                    }
                    else {
                        runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(i - 1 + zs).ToString(), TimeSpan.FromSeconds(1).ToString(), tempOut, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), "-vf fade=in:0:1"));
                    }
                    txt.WriteLine("file '" + tempOut + "'");
                }

                //runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.Remove_StartTime).ToString(), TimeSpan.FromSeconds(2).ToString(), output1, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25),""));

                //runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(2+ zs).ToString(), TimeSpan.FromSeconds(configEntity.CenterTime - 2).ToString(), output4, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), "-vf fade=in:0:1"));

                //runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime+ zs).ToString(), TimeSpan.FromSeconds((inputFile.Duration.TotalSeconds - configEntity.CenterTime)*0.4).ToString(), output5, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25),"-vf fade=in:0:1"));

                //runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime + (inputFile.Duration.TotalSeconds - configEntity.CenterTime)* 0.4+ zs).ToString(), TimeSpan.FromSeconds(inputFile.Duration.TotalSeconds - configEntity.CenterTime).ToString(), output6, "",""));
                //txt.WriteLine("file '" + output1 + "'");
                //txt.WriteLine("file '" + output4 + "'");
                //txt.WriteLine("file '" + output5 + "'");
                //txt.WriteLine("file '" + output6 + "'");
            }
            else
            {
                runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.Remove_StartTime).ToString(), TimeSpan.FromSeconds(2).ToString(), output1, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), ""));

                runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(2).ToString(), TimeSpan.FromSeconds(configEntity.CenterTime - 2).ToString(), output4, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), "-vf fade=in:0:1"));

                runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime).ToString(), TimeSpan.FromSeconds(configEntity.ExtendTime ).ToString(), output5, string.Format(" -filter:v \"setpts={0}*PTS\" ", new Random().Next(2, 6) * 0.25), "-vf fade=in:0:1"));

                runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime).ToString(), TimeSpan.FromSeconds(inputFile.Duration.TotalSeconds - configEntity.Remove_EndTime - configEntity.CenterTime).ToString(), output6, "", ""));
                txt.WriteLine("file '" + output1 + "'");
                txt.WriteLine("file '" + output4 + "'");
                txt.WriteLine("file '" + output5 + "'");
                txt.WriteLine("file '" + output6 + "'");
            }

            
           
            txt.Close();
            runcmd(string.Format("-hwaccel qsv -c:v h264_qsv -noautorotate -f concat -safe 0 -i \"{0}\"  -b:v " + (oldvideo.Bitrate / 1000 + 20) + "k  -c copy -vcodec h264_qsv  \"{1}\" -y", configEntity.LocalPath + @"\FileList.txt", outputpath));
            File.Delete(configEntity.LocalPath + @"\FileList.txt");
        }
        
        /// <summary>
        /// 使用ffmpeg.exe 控制台命令操作视频
        /// </summary>
        /// <param name="code"></param>
        private void runcmd(string code)
        {
            string str = code;

            using (System.Diagnostics.Process p = new System.Diagnostics.Process())
            {
                p.StartInfo.FileName = (configEntity.LocalPath + @"\ffmpeg.exe").Replace("\\\\", "\\");
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.StartInfo.Arguments = str;
                p.Start();//启动程序

                //向cmd窗口发送输入信息
                //p.StandardInput.WriteLine("cd " + path);

                p.StandardInput.WriteLine("&exit");

                p.StandardInput.AutoFlush = true;
                //p.StandardInput.WriteLine("exit");
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令




                //StreamReader reader = p.StandardOutput;
                //string line = reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}
                var output = p.StandardError.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }

        }

        private string SplitVideo(params string[] param)
        {
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            var rui = new Random();
            return string.Format(
                "-hwaccel qsv -noautorotate -ss {1}  -i \"{0}\" -to {2}   -b:v "+ (oldvideo.Bitrate/1000 +20)+ "k -strict experimental -an  {4} {5} " +
                "-vf unsharp=luma_msize_x=7:luma_msize_y=7:luma_amount=" + rui.Next(1,15)*0.1+ " " +//轻度锐化 
                "-vcodec h264_qsv  \"{3}\"", param);
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


        /// <summary>
        /// 视频处理器ffmpeg.exe的位置
        /// </summary>
        public string FFmpegPath { get; set; }
        
    }
}
