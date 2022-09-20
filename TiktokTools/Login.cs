using Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TikTokTools;
using TikTokTools.Util;

namespace TiktokTools
{
    public partial class Login : Form
    {

        public string guid = FingerPrint.Value();
        public Login()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var recode = re_code.Text.Trim();
            WebUtils webUtils = new WebUtils();
            var getResult = webUtils.DoGet("https://fhcollege.com/api/api/TikTookReg?recode="+recode+"&guid=" + guid);
            if (getResult == "true")
            {
                Form1 form1 = new Form1();
                form1.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("注册码错误或已被使用！");
            }

        }
    }
}
