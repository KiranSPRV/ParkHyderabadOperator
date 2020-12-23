using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Xamarin.Essentials;
using HtmlAgilityPack;
using Xamarin.Forms;
using System.Linq;

namespace ParkHyderabadOperator.Model
{
    public class AppVersionServices
    {
        public static string GetAndroidStoreAppVersion()
        {
            string androidAppStoreVersion = null;
            string message = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync("https://play.google.com/store/apps/details?id=" + AppInfo.PackageName + "&hl=en_CA").Result;
                    string localversiomNumber = AppInfo.VersionString;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        var doc = new HtmlDocument();
                        doc.LoadHtml(jsonString);
                        HtmlNodeCollection node = doc.DocumentNode.SelectNodes("//div[@class='BgcNfc']");
                        if (node.Count > 0)
                        {
                            for (var i = 0; i < node.Count; i++)
                            {
                                if (Convert.ToString(node[i].InnerText) == "Current Version")
                                {
                                    androidAppStoreVersion = node[i].NextSibling.InnerText;
                                }
                            }
                        }
                        var versionReuslt = androidAppStoreVersion.CompareTo(localversiomNumber);
                        // 1=App store Version is greaterthan localversion . 0= App store Version is equal to local version,-1= App store Version is lessthan to local version
                        if (versionReuslt==1)        
                        {
                            message="Latest version is avilable on playstore ("+ androidAppStoreVersion + "), your current version is "+ localversiomNumber + "";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                // do something
                Console.WriteLine(ex.Message);
            }
            return message;
        }
        public static string GetIosStoreAppVersion()
        {
            string iOsStoreAppVersion = null;

            string bundleId = AppInfo.PackageName;

            string url = "http://itunes.apple.com/lookup?bundleId=" + bundleId;


            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    string jsonString = webClient.DownloadString(string.Format(url));

                    var lookup = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);


                    if (lookup != null
                        && lookup.Count >= 1
                        && lookup["resultCount"] != null
                        && Convert.ToInt32(lookup["resultCount"].ToString()) > 0)
                    {

                        var results = JsonConvert.DeserializeObject<List<object>>(lookup[@"results"].ToString());


                        if (results != null && results.Count > 0)
                        {
                            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(results[0].ToString());
                            iOsStoreAppVersion = values.ContainsKey("version") ? values["version"].ToString() : string.Empty;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // do something
                Console.WriteLine(ex.Message);
            }

            return iOsStoreAppVersion;
        }
    }
}
