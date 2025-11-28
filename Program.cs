using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TarkovPriceViewer.Configuration;
using TarkovPriceViewer.Models;
using TarkovPriceViewer.Services;
using TarkovPriceViewer.UI;

namespace TarkovPriceViewer
{
    static class Program
    {
        private static IHost _host;
        private static ISettingsService _settingsService;
        private static IBallisticsService _ballisticsService;
        private static ITarkovDataService _tarkovDataService;
        private static ITarkovTrackerService _tarkovTrackerService;
        private static IOcrService _ocrService;

        private static MainForm main = null;
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
        public static readonly String tarkovtracker = "https://tarkovtracker.org/";
        public static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        public static readonly String official = "https://www.escapefromtarkov.com/";
        public static readonly List<String> github = new List<string>() { "https://github.com/hwangshkr/TarkovPriceViewer", "https://github.com/Zotikus1001/TarkovPriceViewer" };
        public static readonly String checkupdate = "/raw/main/README.md";
        public static readonly char rouble = '₽';
        public static readonly char dollar = '$';
        public static readonly char euro = '€';
        public static readonly char[] splitcur = new char[] { rouble, dollar, euro };
        public static readonly Regex inraid_filter = new Regex(@"in raid");
        public static DateTime APILastUpdated = DateTime.Now.AddHours(-5);
        public static DateTime TarkovTrackerAPILastUpdated = DateTime.Now.AddHours(-5);
        public static TarkovAPI.Data tarkovAPI;
        public static TarkovTrackerAPI.Root tarkovTrackerAPI;
        public static bool forceUpdateAPI = false;
        public static bool forceUpdateTrackerAPI = false;
        private static object lockObject = new object();

        public static AppSettings AppSettings => _settingsService.Settings;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            KillDuplicateProcesses();
            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);

            _host = CreateHostBuilder().Build();
            _host.Start();

            _settingsService = _host.Services.GetRequiredService<ISettingsService>();
            _ballisticsService = _host.Services.GetRequiredService<IBallisticsService>();
            _tarkovDataService = _host.Services.GetRequiredService<ITarkovDataService>();
            _tarkovTrackerService = _host.Services.GetRequiredService<ITarkovTrackerService>();
            _ocrService = _host.Services.GetRequiredService<IOcrService>();

            LoadSettings();

            _ = LoadBallisticsDataAsync();
            // Eagerly preload TarkovDev and TarkovTracker so the first scan has all required data
            try
            {
                if (forceUpdateAPI)
                {
                    lock (lockObject)
                    {
                        tarkovAPI = null;
                    }
                }

                _tarkovDataService.UpdateItemListAPI(forceUpdateAPI).GetAwaiter().GetResult();
                forceUpdateAPI = false;

                lock (lockObject)
                {
                    tarkovAPI = _tarkovDataService.Data;
                    APILastUpdated = _tarkovDataService.LastUpdated;
                    finishloadingAPI = _tarkovDataService.IsLoaded;
                }

                Debug.WriteLine("\n--> " + _tarkovDataService.GetLastUpdatedText() + "\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--> Error updating Tarkov API via service at startup: " + ex.Message);
            }

            // Eagerly preload PaddleOCR in the background so the first scan does not need to download/initialize the model,
            // but do not block the main UI thread while the model is being fetched.
            Task.Run(() =>
            {
                try
                {
                    var lang = _settingsService.Settings.Language;
                    if (string.IsNullOrWhiteSpace(lang))
                    {
                        lang = "en";
                    }
                    _ocrService.EnsureInitialized(lang);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("--> Error preloading PaddleOCR at startup (background): " + ex.Message);
                }
            });

            if (AppSettings.UseTarkovTrackerApi)
            {
                try
                {
                    if (forceUpdateTrackerAPI)
                    {
                        lock (lockObject)
                        {
                            tarkovTrackerAPI = null;
                        }
                    }

                    _tarkovTrackerService.UpdateTarkovTrackerAPI(forceUpdateTrackerAPI).GetAwaiter().GetResult();
                    forceUpdateTrackerAPI = false;

                    lock (lockObject)
                    {
                        tarkovTrackerAPI = _tarkovTrackerService.TrackerData;
                        finishloadingTarkovTrackerAPI = _tarkovTrackerService.IsLoaded;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("--> Error updating TarkovTracker API via service at startup: " + ex.Message);
                }
            }

            main = _host.Services.GetRequiredService<MainForm>();

            if (AppSettings.MinimizeToTrayOnStartup)
            {
                Application.Run();
            }
            else
            {
                Application.Run(main);
            }
        }

        private static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddHttpClient();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IBallisticsService, BallisticsService>();
                    services.AddSingleton<ITarkovDataService, TarkovDataService>();
                    services.AddSingleton<ITarkovTrackerService, TarkovTrackerService>();
                    services.AddSingleton<IOcrService, OcrService>();
                    services.AddSingleton<IItemRecognitionService, ItemRecognitionService>();
                    services.AddSingleton<MainForm>();
                });
        }

        private static void KillDuplicateProcesses()
        {
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
        }

        private static async Task LoadBallisticsDataAsync()
        {
            try
            {
                await _ballisticsService.LoadBallisticsAsync();

                lock (lockObject)
                {
                    blist.Clear();
                    foreach (var kv in _ballisticsService.BallisticsData)
                    {
                        blist[kv.Key] = kv.Value;
                    }
                }

                finishloadingballistics = _ballisticsService.IsLoaded;
                Debug.WriteLine("Finished getting Ballistics via service!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading ballistics via service: " + ex.Message);
            }
        }

        public static async void UpdateItemListAPI()
        {
            try
            {
                if (forceUpdateAPI)
                {
                    lock (lockObject)
                    {
                        tarkovAPI = null;
                    }
                }

                await _tarkovDataService.UpdateItemListAPI(forceUpdateAPI);
                forceUpdateAPI = false;

                lock (lockObject)
                {
                    tarkovAPI = _tarkovDataService.Data;
                    APILastUpdated = _tarkovDataService.LastUpdated;
                    finishloadingAPI = _tarkovDataService.IsLoaded;
                }

                Debug.WriteLine("\n--> " + _tarkovDataService.GetLastUpdatedText() + "\n");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--> Error updating Tarkov API via service: " + ex.Message);
            }
        }

        public static async void UpdateTarkovTrackerAPI()
        {
            try
            {
                if (forceUpdateTrackerAPI)
                {
                    lock (lockObject)
                    {
                        tarkovTrackerAPI = null;
                    }
                }

                await _tarkovTrackerService.UpdateTarkovTrackerAPI(forceUpdateTrackerAPI);
                forceUpdateTrackerAPI = false;

                lock (lockObject)
                {
                    tarkovTrackerAPI = _tarkovTrackerService.TrackerData;
                    finishloadingTarkovTrackerAPI = _tarkovTrackerService.IsLoaded;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--> Error updating TarkovTracker API via service: " + ex.Message);
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
                _settingsService.Load();
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
                _settingsService.Save();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error 13: " + e.Message);
            }
        }
    }
}
