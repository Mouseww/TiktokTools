using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public class PostDateClass
    {
        String prop;

        public String Prop
        {
            get { return prop; }
            set { prop = value; }
        }
        String value;

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        /// <summary>
        /// 0为字符串，1为文件
        /// </summary>
        int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        public PostDateClass(String prop, String value, int type = 0)
        {
            this.prop = prop;
            this.value = value;
            this.type = type;
        }


    }
}
