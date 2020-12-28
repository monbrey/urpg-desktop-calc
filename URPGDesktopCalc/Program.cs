using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _calc = new Calc();

            try
            {
                update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to check for updates\n\n{ex.Message}");
            }


            Application.Run(_calc);
        }

        private static async void update()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest req = WebRequest.CreateHttp("https://urpg.monbrey.com.au/calcs/desktop/releases/");
                req.Method = "HEAD";
                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                res?.Close();

                if (res?.StatusCode != HttpStatusCode.OK) return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(@"Unable to check for updates - Remote repository not found.");
            }

            try
            {
                using (UpdateManager mgr = new UpdateManager("https://urpg.monbrey.com.au/calcs/desktop/releases/"))
                {
                    SquirrelAwareApp.HandleEvents(
                        v => mgr.CreateShortcutForThisExe(),
                        v => mgr.CreateShortcutForThisExe(),
                        onAppUninstall: v => mgr.RemoveShortcutForThisExe());

                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (!updateInfo.ReleasesToApply.Any()) return;
                    
                    int versionCount = updateInfo.ReleasesToApply.Count;

                    string versionWord = versionCount > 1 ? "versions" : "version";
                    string message = new StringBuilder().AppendLine($"Calc is {versionCount} {versionWord} behind.").
                        AppendLine("If you choose to update, changes wont take affect until the calc is restarted.").
                        AppendLine("Would you like to download and install them?").
                        ToString();

                    DialogResult result = MessageBox.Show(message, @"Calc update found", MessageBoxButtons.YesNo);
                    if (result != DialogResult.Yes) return;

                    ReleaseEntry updateResult = await mgr.UpdateApp();
                    MessageBox.Show($"Download complete. Version {updateResult.Version} will take effect when App is restarted.", "Calc update complete", MessageBoxButtons.OK);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

