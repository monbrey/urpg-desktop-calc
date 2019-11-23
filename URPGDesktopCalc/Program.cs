using System;
using System.Net;
using System.Windows.Forms;
using CefSharp;
using Squirrel;

namespace URPGDesktopCalc
{
    public static class Program
    {
        private static Calc _calc;

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
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to check for updates\n\n{ex.Message}");
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _calc = new Calc();
            Application.Run(_calc);
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

            try
            {
                using (UpdateManager mgr = new UpdateManager("http://urpg.monbrey.com.au/calcs/desktop/releases/"))
                {
                    string title = _calc.Text;
                    _calc.Text = $"{title} - Downloading update";
                    await mgr.UpdateApp();
                    _calc.Text = $"{title} - Update downloaded and available for next launch";
                    mgr.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

