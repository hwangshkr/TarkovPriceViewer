using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using TarkovPriceViewer.Utils;

namespace TarkovPriceViewer.Configuration
{
    public class AppSettings
    {
        public string Version { get; set; } = AppUtils.GetVersion();
        public bool MinimizeToTrayOnStartup { get; set; } = false;
        public bool CloseOverlayWhenMouseMoved { get; set; } = true;
        public bool RandomItem { get; set; } = false;
        public string ShowOverlayKeyBind { get; set; } = ((int)Keys.F9).ToString();
        public string HideOverlayKeyBind { get; set; } = ((int)Keys.F10).ToString();
        public string CompareOverlayKeyBind { get; set; } = ((int)Keys.F8).ToString();
        public string IncreaseTrackerCountKeyBind { get; set; } = ((int)Keys.Up).ToString();
        public string DecreaseTrackerCountKeyBind { get; set; } = ((int)Keys.Down).ToString();
        public int OverlayTransparent { get; set; } = 80;
        public bool ShowLastPrice { get; set; } = true;
        public bool ShowDayPrice { get; set; } = true;
        public bool ShowWeekPrice { get; set; } = true;
        public bool SellToTrader { get; set; } = true;
        public bool BuyFromTrader { get; set; } = true;
        public bool Needs { get; set; } = true;
        public bool BartersAndCrafts { get; set; } = true;
        public bool UseTarkovTrackerApi { get; set; } = false;
        public string TarkovTrackerApiKey { get; set; } = "APIKey";
        public bool ShowHideoutUpgrades { get; set; } = true;
        public string Language { get; set; } = "en";
        public string Mode { get; set; } = "regular";
        public int ItemWorthThreshold { get; set; } = 7500;
        public int AmmoWorthThreshold { get; set; } = 800;
        public int FleaTraderProfitTolerancePercent { get; set; } = 85;
        public string ToggleFavoriteItemKeyBind { get; set; } = ((int)Keys.F7).ToString();
        public List<string> FavoriteItems { get; set; } = new List<string>();
    }
}
