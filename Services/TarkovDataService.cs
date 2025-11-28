using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TarkovPriceViewer.Models;

namespace TarkovPriceViewer.Services
{
    public interface ITarkovDataService
    {
        TarkovAPI.Data Data { get; }
        DateTime LastUpdated { get; }
        bool IsLoaded { get; }
        Task UpdateItemListAPI(bool force = false);
        string GetLastUpdatedText();
    }

    public class TarkovDataService : ITarkovDataService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ISettingsService _settingsService;
        // Local cache file for Tarkov.dev items data
        private const string API_FILE_PATH = "tarkov-dev-items.json";
        
        public TarkovAPI.Data Data { get; private set; }
        public DateTime LastUpdated { get; private set; } = DateTime.Now.AddHours(-5);
        public bool IsLoaded { get; private set; }
        private readonly object _lockObject = new object();

        public TarkovDataService(IHttpClientFactory httpClientFactory, ISettingsService settingsService)
        {
            _httpClientFactory = httpClientFactory;
            _settingsService = settingsService;
            
            if (File.Exists(API_FILE_PATH))
                LastUpdated = File.GetLastWriteTime(API_FILE_PATH);
        }

        public async Task UpdateItemListAPI(bool force = false)
        {
            var settings = _settingsService.Settings;

            // If Outdated by 15 minutes or forced
            if (force || (DateTime.Now - LastUpdated).TotalMinutes >= 15)
            {
                try
                {
                    AppLogger.Info("TarkovDataService.UpdateItemListAPI", "Updating TarkovDev API...");
                    // Construct query
                    var queryDictionary = new Dictionary<string, string>
                    {
                        { "query", GetGraphQLQuery(settings.Language, settings.Mode) }
                    };

                    var client = _httpClientFactory.CreateClient();
                    var httpResponse = await client.PostAsJsonAsync("https://api.tarkov.dev/graphql", queryDictionary);
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();

                    // Basic cleanup of response if needed (original code did some string manipulation)
                    int index = responseContent.IndexOf("{\"data\":");
                    if (index != -1)
                    {
                        responseContent = responseContent.Remove(index, 8);
                        responseContent = responseContent.Remove(responseContent.Length - 1, 1);
                    }

                    lock (_lockObject)
                    {
                        Data = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                        // Fallback if schema is wrapped
                        if (Data?.items == null)
                        {
                            var temp = JsonConvert.DeserializeObject<ResponseShell>(responseContent);
                            Data = temp?.data;
                        }
                    }

                    LastUpdated = DateTime.Now;
                    IsLoaded = true;
                    AppLogger.Info("TarkovDataService.UpdateItemListAPI", "TarkovDev API Updated");
                    File.WriteAllText(API_FILE_PATH, responseContent);
                }
                catch (Exception ex)
                {
                     AppLogger.Error("TarkovDataService.UpdateItemListAPI", "Error trying to update Tarkov API", ex);
                     // Retry logic could go here, or just let it fail for now
                }
            }
            else if (Data == null)
            {
                LoadFromLocalFile();
            }
            else
            {
                 AppLogger.Info("TarkovDataService.UpdateItemListAPI", "No need to update TarkovDev API! " + GetLastUpdatedText());
            }
        }

        private void LoadFromLocalFile()
        {
            try
            {
                if (File.Exists(API_FILE_PATH))
                {
                    string responseContent = File.ReadAllText(API_FILE_PATH);

                    // Invalidate cache if it does not contain new schema fields
                    if (!responseContent.Contains("foundInRaid") || !responseContent.Contains("\"itemRequirements\":[{\"id\":"))
                    {
                        long size = 0;
                        try
                        {
                            size = new FileInfo(API_FILE_PATH).Length;
                        }
                        catch
                        {
                            // ignore size errors, we'll still log the path
                        }

                        AppLogger.Info("TarkovDataService.LoadFromLocalFile", $"TarkovAPI cache outdated (missing foundInRaid or itemRequirements.id) in '{API_FILE_PATH}' (size={size} bytes), forcing update...");

                        // Force a fresh update and return; caller will deserialize new data.
                        // This method is only called from the async UpdateItemListAPI path (never directly on the UI thread),
                        // so using GetAwaiter().GetResult() here keeps the API of LoadFromLocalFile synchronous without risking UI deadlocks.
                        UpdateItemListAPI(force: true).GetAwaiter().GetResult();
                        return;
                    }
                    lock (_lockObject)
                    {
                        Data = JsonConvert.DeserializeObject<TarkovAPI.Data>(responseContent);
                        if (Data?.items == null)
                        {
                            var temp = JsonConvert.DeserializeObject<ResponseShell>(responseContent);
                            Data = temp?.data;
                        }
                    }
                    AppLogger.Info("TarkovDataService.LoadFromLocalFile", $"TarkovDev API Loaded from local file. {GetLastUpdatedText()}");
                    IsLoaded = true;
                }
            }
            catch (Exception ex)
            {
                long size = 0;
                try
                {
                    if (File.Exists(API_FILE_PATH))
                    {
                        size = new FileInfo(API_FILE_PATH).Length;
                    }
                }
                catch
                {
                    // ignore size errors, we still log the path and exception
                }

                AppLogger.Error("TarkovDataService.LoadFromLocalFile", $"Error trying to load Tarkov API from local file '{API_FILE_PATH}' (size={size} bytes)", ex);
            }
        }

        public string GetLastUpdatedText()
        {
            TimeSpan elapsed = DateTime.Now - LastUpdated;
            if (elapsed.TotalHours < 1)
                return $"Updated: {(int)elapsed.TotalMinutes} minute(s) ago";
            else if (elapsed.TotalDays < 1)
                return $"Updated: {(int)elapsed.TotalHours} hour(s) ago";
            else
                return $"Updated: {(int)elapsed.TotalDays} day(s) ago";
        }

        private string GetGraphQLQuery(string lang, string gameMode)
        {
            return $@"{{
  items(lang:{lang}, gameMode:{gameMode}) {{
    id
    name
    normalizedName
    types
    lastLowPrice
    avg24hPrice
    updated
    fleaMarketFee
    link
    wikiLink
    width
    height
    properties {{
      ... on ItemPropertiesArmor {{
        class
      }}
      ... on ItemPropertiesArmorAttachment {{
        class
      }}
      ... on ItemPropertiesChestRig {{
        class
      }}
      ... on ItemPropertiesGlasses {{
        class
      }}
      ... on ItemPropertiesHelmet {{
        class
      }}
      ... on ItemPropertiesKey {{
        uses
      }}
      ... on ItemPropertiesAmmo {{
        caliber
        damage
        projectileCount
        penetrationPower
        armorDamage
        fragmentationChance
        ammoType
      }}
      ... on ItemPropertiesWeapon {{
        caliber
        ergonomics
        defaultRecoilVertical
        defaultRecoilHorizontal
        defaultWidth
        defaultHeight
        defaultAmmo {{
          name
        }}
      }}
      ... on ItemPropertiesWeaponMod {{
        accuracyModifier
      }}
    }}
    sellFor {{
      currency
      priceRUB
      vendor {{
        name
        ... on TraderOffer {{
          minTraderLevel
        }}
      }}
    }}
    buyFor {{
      currency
      priceRUB
      vendor {{
        name
        ... on TraderOffer {{
          minTraderLevel
        }}
      }}
    }}
    bartersUsing {{
      trader {{
        name
        levels {{
          level
        }}
      }}
      requiredItems {{
        item {{
          name
        }}
        count
        quantity
      }}
      rewardItems {{
        item {{
          name
        }}
        count
        quantity
      }}
    }}
    craftsFor {{
      station {{
        name
        levels {{
          level
        }}
      }}
      requiredItems {{
        item {{
          name
        }}
        count
        quantity
      }}
      rewardItems {{
        item {{
          name
        }}
        count
        quantity
      }}
    }}
    craftsUsing {{
      station {{
        name
        levels {{
          level
        }}
      }}
      requiredItems {{
        item {{
          name
        }}
        count
        quantity
      }}
      rewardItems {{
        item {{
          name
        }}
        count
        quantity
      }}
    }}
    bartersFor {{
      trader {{
        name
        levels {{
          level
        }}
      }}
      requiredItems {{
        item {{
          name
        }}
        count
        quantity
      }}
      rewardItems {{
        item {{
          name
        }}
        count
        quantity
      }}
      taskUnlock {{
        name
      }}
    }}
    usedInTasks {{
      id
      name
      trader {{
        name
      }}
      map {{
        name
      }}
      minPlayerLevel
      traderLevelRequirements {{
        level
      }}
      objectives {{
        id
        description
        maps {{
          name
        }}
        type
        ... on TaskObjectiveItem {{
          id
          count
          type
          foundInRaid
          items {{
            id
            name
          }}
        }}
      }}
    }}
  }}
  hideoutStations {{
    name
    levels {{
      id
      level
      itemRequirements {{
        id
        item {{
          id
          name
        }}
        count
      }}
    }}
  }}
}}";
        }
    }
}
