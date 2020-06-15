using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikTokTools.Util;

namespace TikTokTools.Model
{
    public class VideoInfo
    {
        public VideoInfo() {

        }

        public VideoInfo(Aweme aweme) {
            this.Desc = aweme.Desc==null?Guid.NewGuid().ToString(): aweme.Desc;
            this.AwemeId = aweme.AwemeId;
            this.CommentCount = aweme.Statistics == null ? "0" :aweme.Statistics.CommentCount.ToString();
            this.DiggCount = aweme.Statistics == null ? "0" : aweme.Statistics.DiggCount.ToString();
            this.ForwardCount = aweme.Statistics == null ? "0" : aweme.Statistics.ForwardCount.ToString();
            this.ShareCount = aweme.Statistics == null ? "0" : aweme.Statistics.ShareCount.ToString();
            this.ViewCount= aweme.Statistics == null ? "0" : aweme.Statistics.PlayCount.ToString();
        }
        public string Desc { get; set; }

        public string AwemeId { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public string ShareCount { get; set; }

        /// <summary>
        /// 转发数
        /// </summary>
        public string ForwardCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public string CommentCount { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public string DiggCount { get; set; }


        public string ViewCount { get; set; }

    }
}
