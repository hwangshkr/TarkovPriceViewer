using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TarkovPriceViewer.Configuration;

namespace TarkovPriceViewer.Services
{
    public interface ISettingsService
    {
        AppSettings Settings { get; }
        void Load();
        void Save();
    }

    public class SettingsService : ISettingsService
    {
        private const string SETTING_PATH = "settings.json";
        public AppSettings Settings { get; private set; }

        public SettingsService()
        {
            Settings = new AppSettings();
        }

        public void Load()
        {
            try
            {
                if (!File.Exists(SETTING_PATH))
                {
                    Save(); // Create default
                    return;
                }

                string json = File.ReadAllText(SETTING_PATH);
                Settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception)
            {
                Settings = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(Settings, options);
                File.WriteAllText(SETTING_PATH, json);
            }
            catch (Exception)
            {
                // Log error
            }
        }
    }
}
