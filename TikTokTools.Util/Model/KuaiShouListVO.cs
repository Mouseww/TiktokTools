using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util.Model
{
    public class KuaiShouListVO
    {

        public Data data { get; set; }
    }

    public class Rootobject
    {
        public Data data { get; set; }
    }

    public class Data
    {
        public Visionprofilephotolist visionProfilePhotoList { get; set; }
    }

    public class Visionprofilephotolist
    {
        public int result { get; set; }
        public string llsid { get; set; }
        public string webPageArea { get; set; }
        public Feed[] feeds { get; set; }
        public object hostName { get; set; }
        public string pcursor { get; set; }
        public string __typename { get; set; }
    }

    public class Feed
    {
        public int type { get; set; }
        public Author author { get; set; }
        public Photo photo { get; set; }
        public int canAddComment { get; set; }
        public string llsid { get; set; }
        public int status { get; set; }
        public string currentPcursor { get; set; }
        public string __typename { get; set; }
    }

    public class Author
    {
        public string id { get; set; }
        public string name { get; set; }
        public string headerUrl { get; set; }
        public bool following { get; set; }
        public object headerUrls { get; set; }
        public string __typename { get; set; }
    }

    public class Photo
    {
        public string id { get; set; }
        public int duration { get; set; }
        public string caption { get; set; }
        public string likeCount { get; set; }
        public string viewCount { get; set; }
        public int realLikeCount { get; set; }
        public string coverUrl { get; set; }
        public string photoUrl { get; set; }
        public string photoH265Url { get; set; }
        public Manifest manifest { get; set; }
        public Manifesth265 manifestH265 { get; set; }
        public Videoresource videoResource { get; set; }
        public object coverUrls { get; set; }
        public long timestamp { get; set; }
        public string expTag { get; set; }
        public string animatedCoverUrl { get; set; }
        public object distance { get; set; }
        public float videoRatio { get; set; }
        public bool liked { get; set; }
        public int stereoType { get; set; }
        public bool? profileUserTopPhoto { get; set; }
        public object musicBlocked { get; set; }
        public string __typename { get; set; }
    }

    public class Manifest
    {
        public Playinfo playInfo { get; set; }
        public int mediaType { get; set; }
        public int businessType { get; set; }
        public string version { get; set; }
        public Adaptationset[] adaptationSet { get; set; }
    }

    public class Playinfo
    {
    }

    public class Adaptationset
    {
        public int id { get; set; }
        public int duration { get; set; }
        public Representation[] representation { get; set; }
    }

    public class Representation
    {
        public string url { get; set; }
        public int id { get; set; }
        public string qualityType { get; set; }
        public bool featureP2sp { get; set; }
        public string[] backupUrl { get; set; }
        public int quality { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string qualityLabel { get; set; }
        public int maxBitrate { get; set; }
        public int avgBitrate { get; set; }
        public bool disableAdaptive { get; set; }
        //public int frameRate { get; set; }
        public bool hidden { get; set; }
        public bool defaultSelect { get; set; }
    }

    public class Manifesth265
    {
        public string version { get; set; }
        public int businessType { get; set; }
        public int mediaType { get; set; }
        public string videoId { get; set; }
        public bool hideAuto { get; set; }
        public bool manualDefaultSelect { get; set; }
        public int stereoType { get; set; }
        public Adaptationset1[] adaptationSet { get; set; }
        public Playinfo1 playInfo { get; set; }
        public Videofeature videoFeature { get; set; }
    }

    public class Playinfo1
    {
    }

    public class Videofeature
    {
        public float blurProbability { get; set; }
        public float blockyProbability { get; set; }
        public float avgEntropy { get; set; }
        public float mosScore { get; set; }
    }

    public class Adaptationset1
    {
        public int id { get; set; }
        public int duration { get; set; }
        public Representation1[] representation { get; set; }
    }

    public class Representation1
    {
        public int id { get; set; }
        public string url { get; set; }
        public string[] backupUrl { get; set; }
        public int maxBitrate { get; set; }
        public int avgBitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        //public float frameRate { get; set; }
        public float quality { get; set; }
        public string qualityType { get; set; }
        public string qualityLabel { get; set; }
        public bool featureP2sp { get; set; }
        public bool hidden { get; set; }
        public bool disableAdaptive { get; set; }
        public bool defaultSelect { get; set; }
        public string comment { get; set; }
        public int hdrType { get; set; }
        public int fileSize { get; set; }
        public Kvqscore kvqScore { get; set; }
    }

    public class Kvqscore
    {
        public float FR { get; set; }
        public float NR { get; set; }
    }

    public class Videoresource
    {
        public H264 h264 { get; set; }
        public Hevc hevc { get; set; }
    }

    public class H264
    {
        public Playinfo2 playInfo { get; set; }
        public int mediaType { get; set; }
        public int businessType { get; set; }
        public string version { get; set; }
        public Adaptationset2[] adaptationSet { get; set; }
    }

    public class Playinfo2
    {
    }

    public class Adaptationset2
    {
        public int id { get; set; }
        public int duration { get; set; }
        public Representation2[] representation { get; set; }
    }

    public class Representation2
    {
        public string url { get; set; }
        public int id { get; set; }
        public string qualityType { get; set; }
        public bool featureP2sp { get; set; }
        public string[] backupUrl { get; set; }
        public int quality { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string qualityLabel { get; set; }
        public int maxBitrate { get; set; }
        public int avgBitrate { get; set; }
        public bool disableAdaptive { get; set; }
        //public int frameRate { get; set; }
        public bool hidden { get; set; }
        public bool defaultSelect { get; set; }
    }

    public class Hevc
    {
        public string version { get; set; }
        public int businessType { get; set; }
        public int mediaType { get; set; }
        public string videoId { get; set; }
        public bool hideAuto { get; set; }
        public bool manualDefaultSelect { get; set; }
        public int stereoType { get; set; }
        public Adaptationset3[] adaptationSet { get; set; }
        public Playinfo3 playInfo { get; set; }
        public Videofeature1 videoFeature { get; set; }
    }

    public class Playinfo3
    {
    }

    public class Videofeature1
    {
        public float blurProbability { get; set; }
        public float blockyProbability { get; set; }
        public float avgEntropy { get; set; }
        public float mosScore { get; set; }
    }

    public class Adaptationset3
    {
        public int id { get; set; }
        public int duration { get; set; }
        public Representation3[] representation { get; set; }
    }

    public class Representation3
    {
        public int id { get; set; }
        public string url { get; set; }
        public string[] backupUrl { get; set; }
        public int maxBitrate { get; set; }
        public int avgBitrate { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        //public float frameRate { get; set; }
        public float quality { get; set; }
        public string qualityType { get; set; }
        public string qualityLabel { get; set; }
        public bool featureP2sp { get; set; }
        public bool hidden { get; set; }
        public bool disableAdaptive { get; set; }
        public bool defaultSelect { get; set; }
        public string comment { get; set; }
        public int hdrType { get; set; }
        public int fileSize { get; set; }
        public Kvqscore1 kvqScore { get; set; }
    }

    public class Kvqscore1
    {
        public float FR { get; set; }
        public float NR { get; set; }
    }

}
