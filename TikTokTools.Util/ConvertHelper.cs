using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace TikTokTools.Util
{
    public class ConvertHelper
    {
        public delegate void Run(string msg, string userId = null);
        Run log;
        ConfigEntity configEntity;
        CancellationToken ct;
        IMediaInfo inputFile;
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
                configEntity.SourcePath.Split(',').Where(x=>!string.IsNullOrWhiteSpace(x)).ToList()
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
                try
                {
                    ct.ThrowIfCancellationRequested();
                    log(string.Format("当前正在处理第{0}条", i + 1));
                    string filename = Guid.NewGuid().ToString();
                    string inputpath = files[i];
                    string audiopath = configEntity.LocalPath + @"\Audio\" + filename + ".mp3";
                    string videopath = configEntity.LocalPath + @"\Video\" + filename + ".mp4";
                    string finishpath = configEntity.LocalPath + @"\Finish\" + filename + ".mp4";
                    string output = configEntity.LocalPath + @"\Video\" + filename + "_temp.mp4";
                    inputFile = FFmpeg.GetMediaInfo(inputpath).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    if (inputFile.AudioStreams!=null&&inputFile.AudioStreams.Count()>0)
                    {
                        log("正在抽离音频...");
                        //runcmd(configEntity.LocalPath, string.Format("ffmpeg.exe  -i {0}  -s 1920*1080 {1}", inputpath.Replace(configEntity.LocalPath,""), finishpath.Replace(configEntity.LocalPath, "")));
                        BuildAudio(inputpath, audiopath);
                        ct.ThrowIfCancellationRequested();
                    }
                    else
                    {
                        log("正在临时添加音频...");
                        AddAudio(inputpath, configEntity.LocalPath + @"\Temp.mp3", videopath.Replace(".mp4", "_haveAudio.mp4")).GetAwaiter().GetResult();
                        inputpath = videopath.Replace(".mp4", "_haveAudio.mp4");
                        ct.ThrowIfCancellationRequested();
                    }
                    log("正在进行视频帧处理...");
                    AEVideo(inputpath, output).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    log("正在抽离纯视频...");
                    BuildVideo(output, videopath.Replace(".mp4", "_NoAudio.mp4")).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    inputpath = videopath.Replace(".mp4", "_NoAudio.mp4");
                    log("正在进行调整视频...");
                    AdjustVideo(inputpath, videopath).GetAwaiter().GetResult();
                    ct.ThrowIfCancellationRequested();
                    if (inputFile.AudioStreams != null && inputFile.AudioStreams.Count() > 0)
                    {
                        log("正在将音频加入新的视频中...");
                        AddAudio(videopath, audiopath, finishpath).GetAwaiter().GetResult();
                    }
                    else
                    {
                        log("正在去除音频...");
                        BuildVideo(output, finishpath).GetAwaiter().GetResult();
                        ct.ThrowIfCancellationRequested();
                    }
                    result.Add(finishpath);
                    ct.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    log("出现错误..." + ex.Message);
                    
                }
                finally {
                    log("正在清理临时文件...");
                    clearFolder(configEntity);
                }
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
            var con = await FFmpeg.Conversions.FromSnippet.ExtractAudio(input, audiopath);
            await con.Start();
        }

        private async Task BuildVideo(string input, string videopath)
        {
            var video = await FFmpeg.Conversions.FromSnippet.ExtractVideo(input, videopath);
            await video.Start();
            
            //runcmd(videopath.Substring(0, index), );

        }

        private async Task AdjustVideo(string input, string videopath) {
            log("正在进行裁剪...");
            runcmd(string.Format("-i \"{0}\" -vf crop=iw-iw/100:ih-ih/100:iw/100:ih/100 \"{1}\"", input, videopath.Replace(".mp4", "_tempvideo_Old.mp4")));
            //var conversionResultc = await FFmpeg.Conversions.New().Start(string.Format("-i {0} -vf crop=iw/1.001:ih/1.001:0:0 {1}", input,  videopath));

            log("视频裁剪成功...");

            if (configEntity.Filter || configEntity.Video_Mirroring)
            {
                log("正在调整视频...");
                var outpath = configEntity.Repeat ? videopath.Replace(".mp4", "_tempvideo.mp4") : videopath;
                var conversionResult = await FFmpeg.Conversions.New().Start(string.Format("-i \"{0}\" {1}  {2} \"{3}\" ",
                    videopath.Replace(".mp4", "_tempvideo_Old.mp4"),
                    configEntity.Filter ? string.Format("-vf eq=contrast={0}:brightness={1}:saturation={2}:gamma={3}", configEntity.Contrast,
                    configEntity.Brightness,
                    configEntity.Saturation,
                    configEntity.Gamma) : "",
                    //"crop=iw/1.001:ih/1.001:0:0 ",
                    configEntity.Video_Mirroring ? "-vf hflip" : "",
                    outpath));
                input = outpath;
                log("视频调整完毕...");

            }
            if (configEntity.Repeat)
            {
                log("正在进行画中画...");
                
                var conversionResult = await FFmpeg.Conversions.New().Start(string.Format("-i \"{0}\" -i \"{1}\" -filter_complex overlay=w \"{2}\"", input, input, videopath));

                log("视频画中画成功...");
            }

        }

        private async Task AddAudio(string videopath, string audiopath, string finishpath)
        {
            var random = new Random();
            var video = await FFmpeg.Conversions.FromSnippet.AddAudio(videopath, audiopath, finishpath);
            
            var oldvideo = inputFile.VideoStreams.FirstOrDefault();
            video.SetVideoBitrate(oldvideo.Bitrate + configEntity.Video_BitrateChange);
            //video.SetPixelFormat(PixelFormat.yuyv422);
            video.SetSeek(TimeSpan.FromSeconds(random.Next(1, 10) * 0.01));
            video.SetHashFormat(Hash.SHA512_256);
            video.SetFrameRate(oldvideo.Framerate + configEntity.Video_FrameRateChange);
            video.UseMultiThread(configEntity.ThreadNumber_Single);
            await video.Start();
        }



        private async Task AEVideo(string inputpath, string outputpath)
        {
            string output1 = outputpath.Replace(".mp4", "_temp1.mp4");
            string output2 = outputpath.Replace(".mp4", "_temp2.mp4");
            string output3 = outputpath.Replace(".mp4", "_temp3.mp4");
            string output4 = outputpath.Replace(".mp4", "_temp4.mp4");
            string output5 = outputpath.Replace(".mp4", "_temp5.mp4");
            string output6 = outputpath.Replace(".mp4", "_temp6.mp4");
            IConversion conversion1 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output1, TimeSpan.FromSeconds(configEntity.Remove_StartTime), TimeSpan.FromSeconds(2));
            conversion1.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion1.Start();
            //IConversion conversion2 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output2, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.2));
            //conversion2.UseMultiThread(configEntity.ThreadNumber_Single);
            //await conversion2.Start();
            IConversion conversion3 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output3, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(0.01));
            conversion3.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion3.Start();
            IConversion conversion4 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output4, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1.5));
            conversion4.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion4.Start();
            IConversion conversion5 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output5, TimeSpan.FromSeconds(configEntity.CenterTime), TimeSpan.FromSeconds(configEntity.ExtendTime));
            conversion5.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion5.Start();
            IConversion conversion6 = await FFmpeg.Conversions.FromSnippet.Split(inputpath, output6, TimeSpan.FromSeconds(configEntity.CenterTime), TimeSpan.FromSeconds(inputFile.Duration.TotalSeconds - configEntity.CenterTime - configEntity.Remove_EndTime));
            conversion6.UseMultiThread(configEntity.ThreadNumber_Single);
            await conversion6.Start();
            ConnVideo(outputpath, output1, output3, output4, output5, output6).GetAwaiter().GetResult();
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
    public class Test : FFmpeg
    {

    }
}
