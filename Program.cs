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
        private static MainForm main;
        private static Dictionary<String, String> settings;
        public static readonly List<Item> itemlist = new List<Item>();
        public static readonly String setting_path = @"settings.json";
        public static readonly String appname = "EscapeFromTarkov";
        public static readonly String loading = "Loading...";
        public static readonly String notfound = "Item Name Not Found";
        public static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public static bool MinimizetoTrayWhenStartup = false;
        public static bool CloseOverlayWhenMouseMoved = true;
        public static String ShowOverlay_Key = "F9";
        //public static System.Drawing.Point startpoint = new System.Drawing.Point(0, 0);

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
            if (MinimizetoTrayWhenStartup)
            {
                Application.Run();
            } else
            {
                Application.Run(main);
            }
        }

        static void LoadSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path);
                }
                String text = File.ReadAllText(setting_path);
                settings = JsonSerializer.Deserialize<Dictionary<String, String>>(text);
                foreach (KeyValuePair<String, String> setting in settings)
                {
                    try
                    {
                        switch (setting.Key)
                        {
                            case "MinimizetoTrayWhenStartup":
                                MinimizetoTrayWhenStartup = Convert.ToBoolean(setting.Value);
                                break;
                            case "CloseOverlayWhenMouseMoved":
                                CloseOverlayWhenMouseMoved = Convert.ToBoolean(setting.Value);
                                break;
                            case "ShowOverlay_Key":
                                ShowOverlay_Key = setting.Value;
                                break;
                                /*case "StartPos":
                                    String[] pos = setting.Value.Split(',');
                                    startpoint = new System.Drawing.Point(Int32.Parse(pos[0]), Int32.Parse(pos[1]));
                                    break;*/
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
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
                    item.name_display = spl[0].Trim();//for display '_' removed
                    item.name_compare = item.name_display.ToLower().ToCharArray();//for compare '_' removed
                    item.name_address = spl[1].Trim();//for address '_' not removed
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }
    }
}
