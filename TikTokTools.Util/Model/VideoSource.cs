using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace TikTokTools.Util
{
    public enum VideoSource
    {
        [EnumMember]
        TikTok=1,
        [EnumMember]
        DouYin = 2,
        [EnumMember]
        KuaiShou = 3

    }
}
