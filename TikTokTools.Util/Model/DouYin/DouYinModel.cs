﻿// Generated by ChileQi JSON Class Generator
// QQ：1628983882

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TikTokTools.Util
{

    public class DouYinModel
    {

        [JsonProperty("aweme_list")]
        public IList<Aweme> AwemeList { get; set; }

        [JsonProperty("extra")]
        public Extra Extra { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("max_cursor")]
        public long MaxCursor { get; set; }

        [JsonProperty("min_cursor")]
        public long MinCursor { get; set; }

        [JsonProperty("status_code")]
        public int StatusCode { get; set; }
    }

}
