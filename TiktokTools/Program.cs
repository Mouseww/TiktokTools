using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TiktokTools;
using TikTokTools.Util;

namespace TikTokTools
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string guid = FingerPrint.Value();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WebUtils webUtils = new WebUtils();
            var getResult = webUtils.DoGet("http://www.fhcollege.com/api/api/TikTookLogin?guid=" + guid);
            if (getResult == "true")
            {

                Application.Run(new Form1());
            }
            else { Application.Run(new Login()); }
           
        }
    }
}
