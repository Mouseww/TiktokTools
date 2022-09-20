using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public class ConfigEntity
    {
        public ConfigEntity()
        {
            LocalPath = Environment.CurrentDirectory;
            Remove_StartTime = 0.1;
            Remove_EndTime = 0.1;
            Video_Mirroring = true;
            CenterTime = 3.5;
            ExtendTime = 0.01;
            Video_BitrateChange = (long)200;
            Video_FrameRateChange = 0;
            ThreadNumber_Single = 32;
            Gamma = 1.1;
            Saturation = 1.1;
            Brightness = 0;
            Contrast = 1;
            Repeat = true;
            Filter = true;
            Audio = true;
            AutoCZ = true;
            Crop = 3;
        }

        public VideoSource VideoSource { get; set; }

        public bool IsFilePath { get; set; }

        public string SourcePath { get; set; }

        public string LocalPath { get; set; }

        public double Remove_StartTime { get; set; }

        public double Remove_EndTime { get; set; }

        public double CenterTime { get; set; }

        public double ExtendTime { get; set; }

        public long Video_BitrateChange { get; set; }

        public double Video_FrameRateChange { get; set; }

        public bool Video_Mirroring { get; set; }

        public int ThreadNumber_Single { get; set; }

        public double Gamma { get; set; }

        public double Saturation { get; set; }

        public double Brightness { get; set; }

        public double Contrast { get; set; }

        
        public bool Filter { get; set; }

        public bool Repeat { get; set; }

        public bool Audio { get; set; }

        public bool AutoCZ { get; set; }

        public decimal Crop { get; set; }
    }
}
