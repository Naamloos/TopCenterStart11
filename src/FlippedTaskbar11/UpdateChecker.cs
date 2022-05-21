using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace TopCenterStart11
{
    internal class UpdateChecker
    {
        private GitHubClient client;
        private Config cfg;

        public UpdateChecker(Config cfg)
        {
            client = new GitHubClient(new ProductHeaderValue("Naamloos_TopCenterStart11"));
            this.cfg = cfg;
        }

        public async Task Check()
        {
            IEnumerable<Release> releases = await client.Repository.Release.GetAll("Naamloos", "TopCenterStart11");
            releases = releases.OrderByDescending(x => x.CreatedAt).ToList();
            var latest = releases.First();

            var version = GetType().Assembly.GetName().Version?.ToString() ?? "";

            try
            {
                var versionSplit = version.Split('.').Select(x => int.TryParse(x, out int result) ? (int)result : 0).ToArray();
                var latestSplit = latest.TagName.Split('.').Select(x => int.TryParse(x, out int result) ? (int)result : 0).ToArray();
                var newVersion = true;
                var majorRelease = false;

                if (latest.TagName == cfg.LastCheck)
                    return;

                if (latestSplit[0] != versionSplit[0])
                {
                    majorRelease = true;
                }
                else if (latestSplit[1] != versionSplit[1])
                {
                    majorRelease = false;
                }
                else
                {
                    newVersion = false;
                }

                if(newVersion)
                {
                     var res = MessageBox.Show($"A new {(majorRelease ? "major" : "minor")} version of " +
                         $"TopCenterStart11 is available on GitHub! Do you want to download it right now?\n\n" +
                         $"Skipping this dialog will silence this notification until a new version is available.",
                        "New version available!", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    if(res == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo(latest.HtmlUrl)
                        {
                            UseShellExecute = true
                        });
                }

                cfg.LastCheck = latest.TagName;
                cfg.Save();
            }
            catch (Exception)
            {
                // lazy but works
                return;
            }
        }
    }
}
