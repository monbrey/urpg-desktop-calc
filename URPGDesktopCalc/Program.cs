using System;
using System.Net;
using System.Windows.Forms;
using Squirrel;

namespace URPGDesktopCalc
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            try
            {
                update();
            }
            catch
            {
                MessageBox.Show(@"Unable to check for updates - unknown issue retrieving update files");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Calc());
        }

        private static async void update()
        {
            try
            {
                HttpWebRequest req = WebRequest.CreateHttp("http://urpg.monbrey.com.au/calcs/desktop/releases/");
                req.Method = "HEAD";
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                res?.Close();

                if (res?.StatusCode != HttpStatusCode.OK) return;
            }
            catch
            {
                MessageBox.Show(@"Unable to check for updates - Remote repository not found.");
            }

            using (UpdateManager mgr = new UpdateManager("http://urpg.monbrey.com.au/calcs/desktop/releases/"))
            {
                await mgr.UpdateApp();
            }
        }
    }
}

