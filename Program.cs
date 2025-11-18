using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TarkovPriceViewer
{
    static class Program
    {
        private static MainForm main = null;
        public static Dictionary<String, String> settings = new Dictionary<String, String>();
        public static readonly Dictionary<String, Ballistic> blist = new Dictionary<String, Ballistic>();
        public static readonly Color[] BEColor = new Color[] { ColorTranslator.FromHtml("#B32425"),
            ColorTranslator.FromHtml("#DD3333"),
            ColorTranslator.FromHtml("#EB6C0D"),
            ColorTranslator.FromHtml("#AC6600"),
            ColorTranslator.FromHtml("#FB9C0E"),
            ColorTranslator.FromHtml("#006400"),
            ColorTranslator.FromHtml("#009900") };
        public static readonly HashSet<String> BEType = new HashSet<String> { "Round", "Slug", "Buckshot", "Grenade launcher cartridge" };
        public static readonly String setting_path = @"settings.json";
        public static readonly String appname = "EscapeFromTarkov";
        public static readonly String languageLoading = "Wait Language Model Loading";
        public static readonly String notfound = "Item Name Not Found.";
        public static readonly String waitingForTooltip = "Loading";
        public static readonly String noflea = "Item not Found on the Flea Market.";
        public static readonly String notfinishloading = "Ballistics Data not finished loading. \nPlease try again or check your Internet connection.";
        public static readonly String notfinishloadingAPI = "API not finished loading. \nPlease try again or check your Internet connection.";
        public static readonly String presscomparekey = "Please Press Compare Key.";
        public static bool finishloadingballistics = false;
        public static bool finishloadingAPI = false;
        public static bool finishloadingTarkovTrackerAPI = false;
        public static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly String tarkov_dev = "https://tarkov.dev/";
        public static readonly String tarkovtracker = "https://tarkovtracker.io/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly String official = "https://www.escapefromtarkov.com/";
        public static readonly List<String> github = new List<string>() { "https://github.com/hwangshkr/TarkovPriceViewer", "https://github.com/Zotikus1001/TarkovPriceViewer" };
        public static readonly String checkupdate = "/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public const string WorthPerSlotThresholdKey = "WorthPerSlotThreshold";
        public const int WorthPerSlotThresholdDefault = 7500;
        public static readonly Regex inraid_filter = new Regex(@"in raid");
        public static readonly Regex money_filter = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");
        public static DateTime APILastUpdated = DateTime.Now.AddHours(-5);
        public static DateTime TarkovTrackerAPILastUpdated = DateTime.Now.AddHours(-5);
        public static TarkovAPI.Data tarkovAPI;
        public static TarkovTrackerAPI.Root tarkovTrackerAPI;
        public static bool forceUpdateAPI = false;
        public static bool forceUpdateTrackerAPI = false;
        private static object lockObject = new object();

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            foreach (Process process in Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName))
            {
                if (process.Id == Process.GetCurrentProcess().Id)
                {
                    continue;
                }
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error 15: " + ex.Message);
                }
            }
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);
            Task.Factory.StartNew(() => getBallistics());

            LoadSettings();

            DirectoryInfo di = new DirectoryInfo(@"Resources");
            if (di.Exists == false)
            {
                di.Create();
            }

            if (File.Exists(@"Resources\TarkovAPI.json"))
                APILastUpdated = File.GetLastWriteTime(@"Resources\TarkovAPI.json");

            Task.Factory.StartNew(() => UpdateItemListAPI());

            if (Convert.ToBoolean(settings["useTarkovTrackerAPI"]))
            {
                Task.Factory.StartNew(() => UpdateTarkovTrackerAPI());
            }

            main = new MainForm();
            if (Convert.ToBoolean(settings["MinimizetoTrayWhenStartup"]))
            {
                Application.Run();
            }
            else
            {
                Application.Run(main);
            }
        }

        public static async void UpdateItemListAPI()
        {
            if (forceUpdateAPI)
            {
                lock (lockObject)
                {
                    tarkovAPI = null;
                }
            }
            //If Outdated by 15 minutes.
            if (forceUpdateAPI || (DateTime.Now - APILastUpdated).TotalMinutes >= 15)
            {
                forceUpdateAPI = false;
                try
                {
                    Debug.WriteLine("\n--> Updating API...");

                    var data = new Dictionary<string, string>()
                        {
                            {"query", "{\r\n  items(" + $"lang:{settings["Language"]}, gameMode:{settings["Mode"]}" + ") {\r\n    id\r\n    name\r\n    normalizedName\r\n    types\r\n    lastLowPrice\r\n    avg24hPrice\r\n    updated\r\n    fleaMarketFee\r\n    link\r\n    wikiLink\r\n    width\r\n    height\r\n    properties {\r\n      ... on ItemPropertiesArmor {\r\n        class\r\n      }\r\n      ... on ItemPropertiesArmorAttachment {\r\n        class\r\n      }\r\n      ... on ItemPropertiesChestRig {\r\n        class\r\n      }\r\n      ... on ItemPropertiesGlasses {\r\n        class\r\n      }\r\n      ... on ItemPropertiesHelmet {\r\n        class\r\n      }\r\n      ... on ItemPropertiesKey {\r\n        uses\r\n      }\r\n      ... on ItemPropertiesAmmo {\r\n        caliber\r\n        damage\r\n        projectileCount\r\n        penetrationPower\r\n        armorDamage\r\n        fragmentationChance\r\n        ammoType\r\n      }\r\n      ... on ItemPropertiesWeapon {\r\n        caliber\r\n        ergonomics\r\n        defaultRecoilVertical\r\n        defaultRecoilHorizontal\r\n        defaultWidth\r\n        defaultHeight\r\n        defaultAmmo {\r\n          name\r\n        }\r\n      }\r\n      ... on ItemPropertiesWeaponMod {\r\n        accuracyModifier\r\n      }\r\n    }\r\n    sellFor {\r\n      currency\r\n      priceRUB\r\n      vendor {\r\n        name\r\n        ... on TraderOffer {\r\n          minTraderLevel\r\n        }\r\n      }\r\n    }\r\n    buyFor {\r\n      currency\r\n      priceRUB\r\n      vendor {\r\n        name\r\n        ... on TraderOffer {\r\n          minTraderLevel\r\n        }\r\n      }\r\n    }\r\n    bartersUsing {\r\n      trader {\r\n        name\r\n        levels {\r\n          level\r\n        }\r\n      }\r\n      requiredItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n      rewardItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n    }\r\n    craftsFor {\r\n      station {\r\n        name\r\n        levels {\r\n          level\r\n        }\r\n      }\r\n      requiredItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n      rewardItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n    }\r\n    craftsUsing {\r\n      station {\r\n        name\r\n        levels {\r\n          level\r\n        }\r\n      }\r\n      requiredItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n      rewardItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n    }\r\n    bartersFor {\r\n      trader {\r\n        name\r\n        levels {\r\n          level\r\n        }\r\n      }\r\n      requiredItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n      rewardItems {\r\n        item {\r\n          name\r\n        }\r\n        count\r\n        quantity\r\n      }\r\n      taskUnlock {\r\n        name\r\n      }\r\n    }\r\n    usedInTasks {\r\n      id\r\n      name\r\n      trader {\r\n        name\r\n      }\r\n      map {\r\n        name\r\n      }\r\n      minPlayerLevel\r\n      traderLevelRequirements {\r\n        level\r\n      }\r\n      objectives {\r\n        id\r\n        description\r\n        maps {\r\n          name\r\n        }\r\n        type\r\n        ... on TaskObjectiveItem {\r\n          id\r\n          count\r\n          type\r\n          foundInRaid\r\n          items {\r\n            id\r\n            name\r\n          }\r\n        }\r\n      }\r\n    }\r\n  }\r\n  hideoutStations {\r\n    name\r\n    levels {\r\n      id\r\n      level\r\n      itemRequirements {\r\n        item {\r\n          id\r\n          name\r\n        }\r\n        count\r\n      }\r\n    }\r\n  }\r\n}"}
                        };

                    using (var httpClient = new HttpClient())
                    {
                        //Http response message
                        var httpResponse = await httpClient.PostAsJsonAsync("https://api.tarkov.dev/graphql", data);
                        //Response content
                        string responseContent = await httpResponse.Content.ReadAsStringAsync();

                        int index = responseContent.IndexOf("{\"data\":");
                        if (index != -1)
                        {
                            responseContent = responseContent.Remove(index, 8);
                            responseContent = responseContent.Remove(responseContent.Length - 1, 1);
                        }

                        //Prettify JSON (Produces a larger file)
                        //responseContent = JToken.Parse(responseContent).ToString();

                        lock (lockObject)
                        {
                            tarkovAPI = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                            // Check to make sure the response didn't return an error schema
                            if (tarkovAPI.items == null)
                            {
                                TarkovPriceViewer.ResponseShell temp = JsonConvert.DeserializeObject<TarkovPriceViewer.ResponseShell>(responseContent);
                                tarkovAPI = temp.data;
                            }
                        }
                        APILastUpdated = DateTime.Now;
                        finishloadingAPI = true;
                        Debug.WriteLine("\n--> TarkovDev API Updated!");
                        File.WriteAllText(@"Resources\TarkovAPI.json", responseContent);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("--> Error trying to update Tarkov API: " + ex.Message);
                    Thread.Sleep(5000);
                    UpdateItemListAPI();
                }
            }
            else if (tarkovAPI == null)
            {
                try
                {
                    string responseContent = File.ReadAllText(@"Resources\TarkovAPI.json");

                    if (!responseContent.Contains("foundInRaid"))
                    {
                        Debug.WriteLine("\n--> Cache outdated (missing foundInRaid), forcing update...");
                        forceUpdateAPI = true;
                        UpdateItemListAPI();
                        return;
                    }

                    lock (lockObject)
                    {
                        tarkovAPI = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                        // Check to make sure the response didn't return an error schema
                        if (tarkovAPI.items == null)
                        {
                            TarkovPriceViewer.ResponseShell temp = JsonConvert.DeserializeObject<TarkovPriceViewer.ResponseShell>(responseContent);
                            tarkovAPI = temp.data;
                        }
                    }
                    Debug.WriteLine("\n--> TarkovDev API Loaded from local File! \n--> " + LastUpdated(APILastUpdated) + "\n\n");
                    finishloadingAPI = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("\n--> Error trying to load Tarkov API from local file: " + ex.Message);
                    Thread.Sleep(5000);
                    UpdateItemListAPI();
                }
            }
            else
                Debug.WriteLine("--> No need to update TarkovDev API! \n--> " + LastUpdated(APILastUpdated) + "\n\n");
        }

        public static async void UpdateTarkovTrackerAPI()
        {
            if (forceUpdateTrackerAPI)
            {
                lock (lockObject)
                {
                    tarkovTrackerAPI = null;
                }
            }
            String apiKey = settings["TarkovTrackerAPIKey"];
            if (Convert.ToBoolean(Program.settings["useTarkovTrackerAPI"]) && !string.Equals(apiKey, "APIKey") && !string.IsNullOrWhiteSpace(apiKey))
            {
                //If Outdated by 1 minutes.
                if (forceUpdateTrackerAPI || ((DateTime.Now - TarkovTrackerAPILastUpdated).TotalMinutes >= 1))
                {
                    forceUpdateTrackerAPI = false;
                    try
                    {
                        Debug.WriteLine("\n--> Updating TarkovTracker API...");

                        using (var httpClient = new HttpClient())
                        {
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                            //Http response message
                            var httpResponse = await httpClient.GetAsync("https://tarkovtracker.io/api/v2/progress");
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                //Response content
                                string responseContent = await httpResponse.Content.ReadAsStringAsync();

                                lock (lockObject)
                                {
                                    tarkovTrackerAPI = JsonConvert.DeserializeObject<TarkovTrackerAPI.Root>(responseContent);

                                    TarkovTrackerAPILastUpdated = DateTime.Now;
                                    finishloadingTarkovTrackerAPI = true;
                                    Debug.WriteLine("\n--> TarkovTracker API Updated!");
                                    //File.WriteAllText(@"Resources\TarkovTrackerAPI.json", responseContent);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("--> Error trying to update TarkovTracker API: " + ex.Message);
                        Thread.Sleep(5000);
                        UpdateTarkovTrackerAPI();
                    }
                }
                else
                    Debug.WriteLine("--> No need to update TarkovTracker API! \n--> " + LastUpdated(APILastUpdated) + "\n\n");
            }
        }

        public static string LastUpdated(DateTime time)
        {
            TimeSpan elapsed = DateTime.Now - time;

            if (elapsed.TotalHours < 1)
            {
                if (elapsed.TotalMinutes < 1)
                    return $"Updated: {(int)elapsed.TotalMinutes} minute ago";
                else
                    return $"Updated: {(int)elapsed.TotalMinutes} minutes ago";
            }
            else if (elapsed.TotalDays < 1)
            {
                if (elapsed.TotalHours < 2)
                    return $"Updated: {(int)elapsed.TotalHours} hour ago";
                else
                    return $"Updated: {(int)elapsed.TotalHours} hours ago";
            }
            else
            {
                if (elapsed.TotalDays <= 1)
                    return $"Updated: {(int)elapsed.TotalDays} day ago";
                else
                    return $"Updated: {(int)elapsed.TotalDays} days ago";
            }
        }

        public static void LoadSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path).Dispose();
                }
                String text = File.ReadAllText(setting_path);
                try
                {
                    settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                }
                catch (System.Text.Json.JsonException je)
                {
                    Debug.WriteLine("Error 11: " + je.Message);
                    text = "{}";
                    settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                }
                String st;
                settings.Remove("Version");//force
                settings.Add("Version", "v1.34");//force
                if (!settings.TryGetValue("MinimizetoTrayWhenStartup", out st))
                {
                    settings.Add("MinimizetoTrayWhenStartup", "false");
                }
                if (!settings.TryGetValue("CloseOverlayWhenMouseMoved", out st))
                {
                    settings.Add("CloseOverlayWhenMouseMoved", "true");
                }
                if (!settings.TryGetValue("RandomItem", out st))
                {
                    settings.Add("RandomItem", "false");//false
                }
                if (!settings.TryGetValue("ShowOverlay_Key", out st))
                {
                    settings.Add("ShowOverlay_Key", "120");
                }
                if (!settings.TryGetValue("HideOverlay_Key", out st))
                {
                    settings.Add("HideOverlay_Key", "121");
                }
                if (!settings.TryGetValue("CompareOverlay_Key", out st))
                {
                    settings.Add("CompareOverlay_Key", "119");
                }
                if (!settings.TryGetValue("Overlay_Transparent", out st))
                {
                    settings.Add("Overlay_Transparent", "80");
                }
                if (!settings.TryGetValue("Show_Last_Price", out st))
                {
                    settings.Add("Show_Last_Price", "true");
                }
                if (!settings.TryGetValue("Show_Day_Price", out st))
                {
                    settings.Add("Show_Day_Price", "true");
                }
                if (!settings.TryGetValue("Show_Week_Price", out st))
                {
                    settings.Add("Show_Week_Price", "true");
                }
                if (!settings.TryGetValue("Sell_to_Trader", out st))
                {
                    settings.Add("Sell_to_Trader", "true");
                }
                if (!settings.TryGetValue("Buy_From_Trader", out st))
                {
                    settings.Add("Buy_From_Trader", "true");
                }
                if (!settings.TryGetValue("Needs", out st))
                {
                    settings.Add("Needs", "true");
                }
                if (!settings.TryGetValue("Barters_and_Crafts", out st))
                {
                    settings.Add("Barters_and_Crafts", "true");
                }
                if (!settings.TryGetValue("useTarkovTrackerAPI", out st))
                {
                    settings.Add("useTarkovTrackerAPI", "false");//false
                }
                if (!settings.TryGetValue("TarkovTrackerAPIKey", out st))
                {
                    settings.Add("TarkovTrackerAPIKey", "APIKey");
                }
                if (!settings.TryGetValue("showHideoutUpgrades", out st))
                {
                    settings.Add("showHideoutUpgrades", "true");
                }
                if (!settings.TryGetValue("Language", out st))
                {
                    settings.Add("Language", "en");
                }
                if (!settings.TryGetValue("Mode", out st))
                {
                    settings.Add("Mode", "regular");
                }
                if (!settings.TryGetValue(WorthPerSlotThresholdKey, out st))
                {
                    settings.Add(WorthPerSlotThresholdKey, WorthPerSlotThresholdDefault.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error 12: " + e.Message);
            }
        }

        public static int GetWorthPerSlotThreshold()
        {
            if (settings.TryGetValue(WorthPerSlotThresholdKey, out var value) &&
                int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var threshold))
            {
                return threshold;
            }

            return WorthPerSlotThresholdDefault;
        }

        public static void SaveSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path).Dispose();
                }
                string jsonString = System.Text.Json.JsonSerializer.Serialize<Dictionary<String, String>>(settings);
                File.WriteAllText(setting_path, jsonString.Replace(",", ",\n"));
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error 13: " + e.Message);
            }
        }

        private static void getBallistics()
        {
            while (!finishloadingballistics)
            {
                try
                {
                    using (TPVWebClient wc = new TPVWebClient())
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        Debug.WriteLine(Program.wiki + "Ballistics");
                        doc.LoadHtml(wc.DownloadString(Program.wiki + "Ballistics"));
                        HtmlAgilityPack.HtmlNode node_tm = doc.DocumentNode.SelectSingleNode("//table[4]"); //table[@id='trkballtable']
                        HtmlAgilityPack.HtmlNodeCollection nodes = null;
                        HtmlAgilityPack.HtmlNodeCollection sub_nodes = null;

                        if (node_tm != null)
                        {
                            node_tm = node_tm.SelectSingleNode(".//tbody");
                            if (node_tm != null)
                            {
                                nodes = node_tm.SelectNodes(".//tr");
                                if (nodes != null)
                                {
                                    blist.Clear();
                                    List<Ballistic> sub_blist = new List<Ballistic>();
                                    foreach (HtmlAgilityPack.HtmlNode node in nodes)
                                    {
                                        if (!node.GetAttributeValue("id", "").Equals(""))
                                        {
                                            sub_blist = new List<Ballistic>();
                                        }
                                        sub_nodes = node.SelectNodes(".//td");
                                        if (sub_nodes != null && sub_nodes.Count >= 15)
                                        {
                                            int first = sub_nodes[0].GetAttributeValue("rowspan", 1) == 1 ? 0 : 1;
                                            if (sub_nodes[0].InnerText.Trim().Equals("40x46 mm"))
                                            {
                                                first = 1;
                                            }
                                            String name = sub_nodes[first++].InnerText.Trim();
                                            String special = "";
                                            if (name.EndsWith(" S T"))
                                            {
                                                name = new Regex("(S T)$").Replace(name, "");
                                                special = @"Subsonic & Tracer";
                                            }
                                            if (name.EndsWith(" T"))
                                            {
                                                name = new Regex("T$").Replace(name, "");
                                                special = @"Tracer";
                                            }
                                            if (name.EndsWith(" S"))
                                            {
                                                name = new Regex("S$").Replace(name, "");
                                                special = @"Subsonic";
                                            }
                                            if (name.EndsWith(" S T FM"))
                                            {
                                                name = new Regex("(S T FM)$").Replace(name, "");
                                                special = @"Subsonic & Tracer";
                                            }
                                            if (name.EndsWith(" T FM"))
                                            {
                                                name = new Regex("T FM$").Replace(name, "");
                                                special = @"Tracer";
                                            }
                                            if (name.EndsWith(" S FM"))
                                            {
                                                name = new Regex("S FM$").Replace(name, "");
                                                special = @"Subsonic";
                                            }
                                            if (name.EndsWith(" FM"))//must be last
                                            {
                                                name = new Regex("FM$").Replace(name, "");
                                                special = @"";
                                            }
                                            name = name.Replace("*", "").Trim();
                                            String damage = sub_nodes[first++].InnerText.Trim();
                                            if (damage.Contains("x"))
                                            {
                                                String[] temp_d = damage.Split('x');
                                                int mul = 1;
                                                try
                                                {
                                                    foreach (String d in temp_d)
                                                    {
                                                        mul *= Int32.Parse(d);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine("Error 14: " + ex.Message);
                                                }
                                                damage += " = " + mul;
                                            }
                                            Ballistic b = new Ballistic(
                                                name
                                                , damage
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , sub_nodes[first++].InnerText.Trim()
                                                , special
                                                , sub_blist
                                                );
                                            sub_blist.Add(b);
                                            blist.Add(name, b);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    finishloadingballistics = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error with Ballistics : " + e.Message);
                    Thread.Sleep(3000);
                }
            }
            Debug.WriteLine("Finished getting Ballistics!");
        }
    }
}
