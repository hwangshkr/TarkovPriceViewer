using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TarkovPriceViewer
{
    static class Program
    {
        public static bool UsingAPI = true;
 
        private static MainForm main = null;
        public static Dictionary<String, String> settings = new Dictionary<String, String>();
        public static readonly List<Item> itemlist = new List<Item>();
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
        public static readonly String loading = "Loading...";
        public static readonly String notfound = "Item Name Not Found.";
        public static readonly String waitingForTooltip = "Waiting for tooltip";
        public static readonly String noflea = "Item not Found on the Flea Market.";
        public static readonly String notfinishloading = "Ballistics Data not finished loading. \nPlease try again or check your Internet connection.";
        public static readonly String notfinishloadingAPI = "API not finished loading. \nPlease try again or check your Internet connection.";
        public static readonly String presscomparekey = "Please Press Compare Key.";
        public static bool finishloadingballistics = false;
        public static bool finishloadingAPI = false;
        public static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly String tarkov_dev = "https://tarkov.dev/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly String official = "https://www.escapefromtarkov.com/";
        public static readonly String github = "https://github.com/Zotikus1001/TarkovPriceViewer";
        public static readonly String checkupdate = "https://github.com/Zotikus1001/TarkovPriceViewer/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public static readonly Regex inraid_filter = new Regex(@"in raid");
        public static readonly Regex money_filter = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");
        public static DateTime APILastUpdated = DateTime.Now.AddHours(-5);
        public static TarkovAPI.Data tarkovAPI;

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
            Task task = Task.Factory.StartNew(() => getBallistics());

            LoadSettings();

            if (UsingAPI)
            {
                if (File.Exists(@"Resources\TarkovAPI.json"))
                    APILastUpdated = File.GetLastWriteTime(@"Resources\TarkovAPI.json");

                Task task2 = Task.Factory.StartNew(() => UpdateItemListAPI());
            }
            else
                getItemList();

            main = new MainForm();
            if (Convert.ToBoolean(settings["MinimizetoTrayWhenStartup"]))
            {
                Application.Run();
            } else
            {
                Application.Run(main);
            }
        }

        private static void getItemList()
        {
            String[] textValue = null;
            if (File.Exists(@"Resources\itemlist.txt"))
            {
                textValue = File.ReadAllLines(@"Resources\itemlist.txt");
            }
            if (textValue != null && textValue.Length > 0)
            {
                for (int i = 0; i < textValue.Length; i++)//ignore 1,2 Line
                {
                    String[] spl = textValue[i].Split('\t');
                    Item item = new Item();
                    item.name_display = spl[0].Trim(); //Column 1
                    item.name_display2 = spl[2].Trim(); //Column 3
                    item.name_compare = item.name_display.ToLower().ToCharArray();
                    item.name_compare2 = item.name_display2.ToLower().ToCharArray();
                    item.market_address = spl[1].Replace(" ", "_").Trim(); //Column 2
                    item.wiki_address = spl[0].Replace(" ", "_").Trim(); //Column 1
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }

        public static async void UpdateItemListAPI()
        {
            if (UsingAPI)
            {
                //If Outdated by 15 minutes.
                if ((DateTime.Now - APILastUpdated).TotalMinutes >= 15)
                {
                    try
                    {
                        var data = new Dictionary<string, string>()
                    {
                        {"query", "{\r\n  items {\r\n    name\r\n    types\r\n    lastLowPrice\r\n    avg24hPrice\r\n    updated\r\n    fleaMarketFee\r\n    link\r\n    wikiLink\r\n    width\r\n    height\r\n    properties {\r\n      ... on ItemPropertiesAmmo {\r\n        caliber\r\n        damage\r\n        projectileCount\r\n        penetrationPower\r\n        armorDamage\r\n        fragmentationChance\r\n        ammoType\r\n      }\r\n      ... on ItemPropertiesWeapon {\r\n        caliber\r\n        ergonomics\r\n        defaultRecoilVertical\r\n        defaultRecoilHorizontal\r\n        defaultWidth\r\n        defaultHeight\r\n        defaultAmmo {\r\n          name\r\n        }\r\n      }\r\n    }\r\n    sellFor {\r\n      currency\r\n      priceRUB\r\n      vendor {\r\n        name\r\n        ... on TraderOffer {\r\n          minTraderLevel\r\n        }\r\n      }\r\n    }\r\n    buyFor {\r\n      currency\r\n      priceRUB\r\n      vendor {\r\n        name\r\n        ... on TraderOffer {\r\n          minTraderLevel\r\n        }\r\n      }\r\n    }\r\n    usedInTasks {\r\n      name\r\n      trader {\r\n        name\r\n      }\r\n      map {\r\n        name\r\n      }\r\n      minPlayerLevel\r\n      traderLevelRequirements {\r\n        level\r\n      }\r\n    }\r\n  }\r\n}"}
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

                            tarkovAPI = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                            File.WriteAllText(@"Resources\TarkovAPI.json", responseContent);
                            Debug.WriteLine("\n--> API Updated!");
                            APILastUpdated = DateTime.Now;
                            finishloadingAPI = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("--> Error trying to update Tarkov API: " + ex.Message);
                    }
                }
                else if (tarkovAPI == null)
                {
                    try
                    {
                        string responseContent = File.ReadAllText(@"Resources\TarkovAPI.json");
                        tarkovAPI = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                        Debug.WriteLine("\nAPI Loaded from local File! \n" + LastUpdated(APILastUpdated));
                        finishloadingAPI = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("\n--> Error trying to load Tarkov API from local file: " + ex.Message);
                    }
                }
                else
                    Debug.WriteLine("\n--> No need to update API! \n--> " + LastUpdated(APILastUpdated));
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
                if(elapsed.TotalDays <= 1) 
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
                } catch (System.Text.Json.JsonException je)
                {
                    Debug.WriteLine("Error 11: " + je.Message);
                    text = "{}";
                    settings = System.Text.Json.JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                }
                String st;
                settings.Remove("Version");//force
                settings.Add("Version", "v1.21");//force
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
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error 12: " + e.Message);
            }
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
