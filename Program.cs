using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TarkovPriceViewer
{
    static class Program
    {
        private static MainForm main = null;
        public static Dictionary<String, String> settings = new Dictionary<String, String>();
        public static readonly List<Item> itemlist = new List<Item>();
        public static readonly String setting_path = @"settings.json";
        public static readonly String appname = "EscapeFromTarkov";
        public static readonly String loading = "Loading...";
        public static readonly String notfound = "Item Name Not Found";
        public static readonly String[] traderlist = { "Prapor", "Therapist", "Skier", "Peacekeeper", "Mechanic", "Ragman", "Jaeger" };//Fence
        public static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly String github = "https://github.com/hwangshkr/TarkovPriceViewer";
        public static readonly String checkupdate = "https://github.com/hwangshkr/TarkovPriceViewer/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };

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

        static void getItemList()
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
                    item.name_compare = item.name_display.ToLower().ToCharArray();
                    item.name_address = spl[1].Replace(" ", "_").Trim();
                    item.name_display2 = spl[2].Trim();
                    item.name_compare2 = item.name_display2.ToLower().ToCharArray();
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }

        static void LoadSettings()
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
                settings.Add("Version", "v1.01");//force
                if (!settings.TryGetValue("MinimizetoTrayWhenStartup", out st))
                {
                    settings.Add("MinimizetoTrayWhenStartup", "false");
                }
                if (!settings.TryGetValue("CloseOverlayWhenMouseMoved", out st))
                {
                    settings.Add("CloseOverlayWhenMouseMoved", "true");
                }
                if (!settings.TryGetValue("ShowOverlay_Key", out st))
                {
                    settings.Add("ShowOverlay_Key", "120");
                }
                if (!settings.TryGetValue("HideOverlay_Key", out st))
                {
                    settings.Add("HideOverlay_Key", "121");
                }
                if (!settings.TryGetValue("Overlay_Transparent", out st))
                {
                    settings.Add("Overlay_Transparent", "80");
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
                File.WriteAllText(setting_path, jsonString);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
