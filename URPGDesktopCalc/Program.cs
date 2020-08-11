using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
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
                Task.Run(update);
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
                    SquirrelAwareApp.HandleEvents(
                        v => mgr.CreateShortcutForThisExe(),
                        v => mgr.CreateShortcutForThisExe(),
                        onAppUninstall: v => mgr.RemoveShortcutForThisExe());

                    await mgr.UpdateApp();
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

