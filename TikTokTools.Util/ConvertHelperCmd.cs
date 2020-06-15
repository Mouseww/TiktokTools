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

            //ParallelOptions options = new ParallelOptions();
            //options.MaxDegreeOfParallelism = 4;
            //int index = 0;
            //log(string.Format("开始多线程处理..."));
            //Parallel.ForEach(files, (item) =>
            //{
            //    //log(string.Format("当前正在处理第{0}条", index ));
            //    string filename = Guid.NewGuid().ToString();
            //    string audiopath = LocalPath + @"\Audio\" + filename + ".mp3";
            //    string videopath = LocalPath + @"\Video\" + filename + ".mp4";
            //    string finishpath = LocalPath + @"\Finish\" + filename + ".mp4";
            //    string output = LocalPath + @"\Video\" + filename + "_temp.mp4";
            //    //log("正在抽离音频...");
            //    BuildAudio(item, audiopath).GetAwaiter().GetResult();
            //    //log("正在处理视频...");
            //    AEVideo(item, output).GetAwaiter().GetResult();
            //    //log("正在去除视频中的音频...");
            //    BuildVideo(output, videopath).GetAwaiter().GetResult();
            //    //log("正在将音频加入新的视频中...");
            //    AddAudio(videopath, audiopath, finishpath).GetAwaiter().GetResult();
            //    File.Delete(videopath.Replace(".mp4", "_tempvideo.mp4"));
            //    index++;
            //    log(string.Format("已处理{0}条", index));
            //});
            for (int i = 0; i < files.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    log(string.Format("当前正在处理第{0}条", i + 1));
                    Match match = Regex.Match(files[i], @".+\\(.+)");
                    string filename = match.Groups[1].Value;
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
                    else
                    {
                        //log("正在临时添加音频...");

                        //AddAudio(inputpath, configEntity.LocalPath + @"\Temp.mp3", videopath.Replace(".mp4", "_haveAudio.mp4")).GetAwaiter().GetResult();
                        //inputpath = videopath.Replace(".mp4", "_haveAudio.mp4");
                        //ct.ThrowIfCancellationRequested();
                    }
                    log("正在进行视频帧处理...");
                    AEVideo(inputpath, output).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    //log("正在抽离纯视频...");
                    inputpath = output;
                    //BuildVideo(inputpath, videopath.Replace(".mp4", "_NoAudio.mp4")).GetAwaiter().GetResult();
                    //ct.ThrowIfCancellationRequested();
                    log("正在进行调整视频...");
                    //inputpath = videopath.Replace(".mp4", "_NoAudio.mp4");
                    AdjustVideo(inputpath, videopath).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    inputpath = videopath;
                    if (inputFile.AudioStreams != null && inputFile.AudioStreams.Count() > 0)
                    {
                        log("正在将音频加入新的视频中...");
                        videopath = videopath.Replace(".mp4", "_Audio.mp4");
                        AddAudio(inputpath, audiopath, videopath).GetAwaiter().GetResult();

                        //runcmd(string.Format(" ffmpeg -i {0} -i {1} -vcodec copy -acodec copy {2}", inputpath, audiopath, videopath.Replace(".mp4", "_haveAudio.mp4")));
                        //AddAudio(videopath, audiopath, finishpath).GetAwaiter().GetResult();
                    }
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
            //runcmd(string.Format("-i \"{0}\" -c:a aac -vn \"{1}\"", input, audiopath));
            var video = await FFmpeg.Conversions.FromSnippet.ExtractAudio(input, audiopath);
            await video.Start();
        }

        private async Task BuildVideo(string input, string videopath)
        {
            runcmd(string.Format(" -i \"{0}\" -vcodec copy -an \"{1}\"", input, videopath));
            //var video = await FFmpeg.Conversions.FromSnippet.ExtractVideo(input, videopath.Replace(".mp4", "_tempvideo_Old.mp4"));
            //await video.Start();

            //runcmd(videopath.Substring(0, index), );

        }

        private async Task AdjustVideo(string input, string videopath)
        {
            if (configEntity.Filter || configEntity.Video_Mirroring)
            {
                log("正在调整视频...");
                var outpath = configEntity.Repeat ? videopath.Replace(".mp4", "_tempvideo.mp4") : videopath;
                runcmd(string.Format("-i \"{0}\" {1}  {2}  \"{3}\" ",
                   input,
                   configEntity.Filter ? string.Format("-vf eq=contrast={0}:brightness={1}:saturation={2}:gamma={3}", configEntity.Contrast,
                   configEntity.Brightness,
                   configEntity.Saturation,
                   configEntity.Gamma) : "",
                   configEntity.Video_Mirroring ? "-vf hflip" : "",
                   outpath));
                input = outpath;
                log("视频调整完毕...");

            }
            if (configEntity.Repeat)
            {
                log("正在进行画中画...");
                runcmd(string.Format("-i \"{0}\" -i \"{1}\" -filter_complex  overlay \"{2}\" -y", input, input, videopath));

                log("视频画中画成功...");
            }
        }

        private async Task AddAudio(string videopath, string audiopath, string finishpath)
        {
            runcmd(string.Format("-i \"{0}\" -i \"{1}\" -vcodec copy -acodec copy \"{2}\" -y", videopath, audiopath, finishpath));

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

        private async Task changeVideo(string inputpath, string outpath)
        {
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            runcmd(string.Format("-i \"{0}\" -b:v {1}k -s 720*1280 -r {2} -vf crop=iw-iw/100:ih-ih/100:iw/100:ih/100 \"{3}\" -y", inputpath, oldvideo.Bitrate / 1000 + configEntity.Video_BitrateChange, oldvideo.Framerate + configEntity.Video_FrameRateChange, outpath));
        }

        private async Task AEVideo(string inputpath, string outputpath)
        {
            string output1 = outputpath.Replace(".mp4", "_temp1.mp4");
            string output2 = outputpath.Replace(".mp4", "_temp2.mp4");
            string output3 = outputpath.Replace(".mp4", "_temp3.mp4");
            string output4 = outputpath.Replace(".mp4", "_temp4.mp4");
            string output5 = outputpath.Replace(".mp4", "_temp5.mp4");
            string output6 = outputpath.Replace(".mp4", "_temp6.mp4");
            runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.Remove_StartTime).ToString(), TimeSpan.FromSeconds(2.01).ToString(), output1));
            //IConversion conversion1 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output1, TimeSpan.FromSeconds(configEntity.Remove_StartTime), TimeSpan.FromSeconds(2));
            //conversion1.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion1.Start();
            //IConversion conversion2 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output2, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.2));
            //conversion2.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion2.Start();
            //runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(2).ToString(), TimeSpan.FromSeconds(0.01).ToString(), output3));
            //IConversion conversion3 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.01));
            //conversion3.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion3.Start();
            runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(2).ToString(), TimeSpan.FromSeconds(configEntity.CenterTime-2).ToString(), output4));
            //IConversion conversion4 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output4, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1.5));
            //conversion4.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion4.Start();
            runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime).ToString(), TimeSpan.FromSeconds(configEntity.ExtendTime).ToString(), output5));
            //IConversion conversion5 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output5, TimeSpan.FromSeconds(configEntity.CenterTime), TimeSpan.FromSeconds(configEntity.ExtendTime));
            //conversion5.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion5.Start();
            runcmd(SplitVideo(inputpath, TimeSpan.FromSeconds(configEntity.CenterTime).ToString(), TimeSpan.FromSeconds(inputFile.Duration.TotalSeconds-  configEntity.Remove_EndTime- configEntity.CenterTime).ToString(), output6));

            //IConversion conversion6 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output6, TimeSpan.FromSeconds(configEntity.CenterTime), TimeSpan.FromSeconds(inputFile.Duration.TotalMilliseconds - configEntity.CenterTime - configEntity.Remove_EndTime));
            //conversion6.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion6.Start();
            var txt = File.CreateText(configEntity.LocalPath + @"\FileList.txt");
            txt.WriteLine("file '" + output1 + "'");
            //txt.WriteLine("file '" + output3 + "'");
            txt.WriteLine("file '" + output4 + "'");
            txt.WriteLine("file '" + output5 + "'");
            txt.WriteLine("file '" + output6 + "'");
            txt.Close();
            runcmd(string.Format("-f concat -safe 0 -i \"{0}\"  -c copy \"{1}\" -y", configEntity.LocalPath + @"\FileList.txt", outputpath));
            File.Delete(configEntity.LocalPath + @"\FileList.txt");
            //ConnVideo(outputpath, output1, output3, output4, output5, output6).GetAwaiter().GetResult();
        }

        private async Task ConnVideo(string outputpath, params string[] inputpath)
        {
            var conversion = await FFmpeg.Conversions.FromSnippet.Concatenate(outputpath, inputpath);
            conversion.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion.Start();
        }

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

            return string.Format("-ss {1}  -i \"{0}\" -to {2} -c:v libx264  -strict experimental -an \"{3}\"", param);
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

        /// <summary>
        /// 调用ffmpeg.exe 执行命令
        /// </summary>
        /// <param name="Parameters">命令参数</param>
        /// <returns>返回执行结果</returns>
        private string RunProcess(string Parameters)
        {
            //创建一个ProcessStartInfo对象 并设置相关属性
            var oInfo = new ProcessStartInfo(FFmpegPath, Parameters);
            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;
            oInfo.RedirectStandardInput = true;

            //创建一个字符串和StreamReader 用来获取处理结果
            string output = null;
            StreamReader srOutput = null;

            try
            {
                //调用ffmpeg开始处理命令
                var proc = Process.Start(oInfo);
                //获取输出流
                srOutput = proc.StandardError;

                //转换成string
                output = srOutput.ReadToEnd();
                proc.WaitForExit();




                //关闭处理程序
                proc.Close();
            }
            catch (Exception)
            {
                output = string.Empty;
            }
            finally
            {
                //释放资源
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
            return output;
        }
    }
}
