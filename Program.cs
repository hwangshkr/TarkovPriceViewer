using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
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
        public static readonly List<Item> itemlist = new List<Item>();
        public static readonly Dictionary<String, Ballistic> blist = new Dictionary<String, Ballistic>();
        public static readonly Color[] BEColor = new Color[] { ColorTranslator.FromHtml("#B32425"),
            ColorTranslator.FromHtml("#DD3333"),
            ColorTranslator.FromHtml("#EB6C0D"),
            ColorTranslator.FromHtml("#AC6600"),
            ColorTranslator.FromHtml("#FB9C0E"),
            ColorTranslator.FromHtml("#006400"),
            ColorTranslator.FromHtml("#009900") };
        public static readonly String setting_path = @"settings.json";
        public static readonly String appname = "EscapeFromTarkov";
        public static readonly String loading = "Loading...";
        public static readonly String notfound = "Item Name Not Found.";
        public static readonly String noflea = "Item not Found on Flea.";
        public static readonly String notfinishloading = "Wait for Loading Data. Please Check Your Internet, and Check Tarkov Wiki Site.";
        public static readonly String presscomparekey = "Please Press Compare Key.";
        public static bool finishloadingballistics = false;
        public static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly String official = "https://www.escapefromtarkov.com/";
        public static readonly String github = "https://github.com/hwangshkr/TarkovPriceViewer";
        public static readonly String checkupdate = "https://github.com/hwangshkr/TarkovPriceViewer/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public static readonly Regex inraid_filter = new Regex(@"in raid");
        public static readonly Regex money_filter = new Regex(@"([\d,]+[₽\$€]|[₽\$€][\d,]+)");

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
                    Debug.WriteLine(ex.Message);
                }
            }
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);
            Task task = Task.Factory.StartNew(() => getBallistics());
            LoadSettings();
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
                    item.name_display = spl[0].Trim();
                    item.name_display2 = spl[2].Trim();
                    item.name_compare = item.name_display.ToLower().ToCharArray();
                    item.name_compare2 = item.name_display2.ToLower().ToCharArray();
                    item.market_address = spl[1].Replace(" ", "_").Trim();
                    item.wiki_address = spl[0].Replace(" ", "_").Trim();
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
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
                    settings = JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                } catch (JsonException je)
                {
                    Debug.WriteLine(je.Message);
                    text = "{}";
                    settings = JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                }
                String st;
                settings.Remove("Version");//force
                settings.Add("Version", "v1.10");//force
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
                Debug.WriteLine(e.Message);
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
                string jsonString = JsonSerializer.Serialize<Dictionary<String, String>>(settings);
                File.WriteAllText(setting_path, jsonString.Replace(",", ",\n"));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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
                        HtmlAgilityPack.HtmlNode node_tm = doc.DocumentNode.SelectSingleNode("//table[@id='trkballtable']");
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
                                                    Debug.WriteLine(ex.Message);
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
                    Debug.WriteLine("error with ballistics : " + e.Message);
                    Thread.Sleep(3000);
                }
            }
            Debug.WriteLine("finish to get ballistics.");
        }
    }
}
