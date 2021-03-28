using Lsj.Util.JSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowDebugger
{
    public static class UpdateHelper
    {
        public static async Task CheckUpdate()
        {
            try
            {
                var response = await new HttpClient().GetAsync("https://gitlab.sdlsj.net/api/v4/projects/28/releases");//Gitlab release api
                if (response.IsSuccessStatusCode)
                {
                    var latestReleaseVersion = (JSONParser.Parse<List<ReleaseItem>>(await response.Content.ReadAsStringAsync())).Max(x => x.ReleaseVersion);
                    if (Assembly.GetExecutingAssembly().GetName().Version < latestReleaseVersion)
                    {
                        if (MessageBox.Show("检测到更新版本，是否前往下载", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://gitlab.sdlsj.net/lsj/windowdebugger/-/releases/",
                                UseShellExecute = true,
                            });
                        }
                    }
                }
            }
            catch
            {
                //Ignore exception for check update
            }
        }

        public class ReleaseItem
        {
            [CustomJsonPropertyName("name")]
            public string Name { get; set; }

            [NotSerialize]
            public Version ReleaseVersion => Version.TryParse(Name, out var result) ? result : new Version();

        }
    }
}
