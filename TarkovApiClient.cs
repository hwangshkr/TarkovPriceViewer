using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TarkovPriceViewer
{
    // GraphQL client kept as a fallback; uses several small queries instead
    // of one giant one so parts get cached server-side and retried
    // independently.
    public static class TarkovApiClient
    {
        public const string ApiUrl = "https://api.tarkov.dev/graphql";

        private const string PropertiesBlock =
            "properties { " +
            "... on ItemPropertiesArmor { class } " +
            "... on ItemPropertiesArmorAttachment { class } " +
            "... on ItemPropertiesChestRig { class } " +
            "... on ItemPropertiesGlasses { class } " +
            "... on ItemPropertiesHelmet { class } " +
            "... on ItemPropertiesKey { uses } " +
            "... on ItemPropertiesAmmo { caliber damage projectileCount penetrationPower armorDamage fragmentationChance ammoType } " +
            "... on ItemPropertiesWeapon { caliber ergonomics defaultRecoilVertical defaultRecoilHorizontal defaultWidth defaultHeight defaultAmmo { name } } " +
            "... on ItemPropertiesWeaponMod { accuracyModifier } " +
            "}";

        private const string TradeBlock =
            "requiredItems { item { name } count quantity } " +
            "rewardItems { item { name } count quantity }";

        // __ARGS__ is replaced with e.g. "lang:en, gameMode:regular" at runtime.
        public static readonly string[] QueryParts = new string[]
        {
            // 1. Base item info + properties
            "{ items(__ARGS__) { id name normalizedName types lastLowPrice avg24hPrice updated fleaMarketFee link wikiLink width height " + PropertiesBlock + " } }",

            // 2. Trader prices
            "{ items(__ARGS__) { id " +
            "sellFor { currency priceRUB vendor { name ... on TraderOffer { minTraderLevel } } } " +
            "buyFor { currency priceRUB vendor { name ... on TraderOffer { minTraderLevel } } } " +
            "} }",

            // 3. Barters
            "{ items(__ARGS__) { id " +
            "bartersUsing { trader { name levels { level } } " + TradeBlock + " } " +
            "bartersFor { trader { name levels { level } } " + TradeBlock + " taskUnlock { name } } " +
            "} }",

            // 4. Crafts
            "{ items(__ARGS__) { id " +
            "craftsFor { station { name levels { level } } " + TradeBlock + " } " +
            "craftsUsing { station { name levels { level } } " + TradeBlock + " } " +
            "} }",

            // 5. Tasks
            "{ items(__ARGS__) { id " +
            "usedInTasks { " +
            "objectives { id description maps { name } ... on TaskObjectiveItem { optional type count items { id name } } } " +
            "id name trader { name } map { name } minPlayerLevel traderLevelRequirements { level } " +
            "} " +
            "} }",

            // 6. Hideout stations
            "{ hideoutStations { name levels { id level itemRequirements { item { id name } count attributes { type value } } } } }"
        };

        public static async Task<TarkovAPI.Data> FetchAll(string language, string gameMode)
        {
            // json.tarkov.dev is the primary source; GraphQL is a fallback.
            try
            {
                Debug.WriteLine("--> Fetching data from json.tarkov.dev...");
                return await TarkovJsonApiClient.FetchAll(language, gameMode);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--> json.tarkov.dev failed: " + ex.Message + "; falling back to the GraphQL API");
                return await FetchAllGraphQL(language, gameMode);
            }
        }

        public static async Task<TarkovAPI.Data> FetchAllGraphQL(string language, string gameMode)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "TarkovPriceViewer");
                string args = "lang:" + language + ", gameMode:" + gameMode;
                List<TarkovAPI.Data> parts = new List<TarkovAPI.Data>();
                for (int i = 0; i < QueryParts.Length; i++)
                {
                    Debug.WriteLine("--> Fetching tarkov.dev API part " + (i + 1) + "/" + QueryParts.Length + "...");
                    string query = QueryParts[i].Replace("__ARGS__", args);
                    parts.Add(await FetchPart(client, query));
                }
                TarkovAPI.Data merged = Merge(parts);
                if (merged == null || merged.items == null)
                {
                    throw new Exception("API responses contained no item data");
                }
                return merged;
            }
        }

        private static async Task<TarkovAPI.Data> FetchPart(HttpClient client, string query)
        {
            const int maxAttempts = 3;
            Exception lastError = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    StringContent body = new StringContent(
                        JsonConvert.SerializeObject(new Dictionary<string, string> { { "query", query } }),
                        Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(ApiUrl, body);
                    string responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("tarkov.dev API returned HTTP "
                            + (int)response.StatusCode + " " + response.StatusCode
                            + ExtractServerErrors(responseContent));
                    }
                    ResponseShell shell = JsonConvert.DeserializeObject<ResponseShell>(responseContent);
                    if (shell == null || shell.data == null
                        || (shell.data.items == null && shell.data.hideoutStations == null))
                    {
                        throw new Exception("API response contained no data" + ExtractServerErrors(responseContent));
                    }
                    return shell.data;
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    Debug.WriteLine("--> API part attempt " + attempt + "/" + maxAttempts + " failed: " + ex.Message);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5 * attempt));
                    }
                }
            }
            throw lastError;
        }

        // Combines partial responses into one TarkovAPI.Data, matching items by id.
        public static TarkovAPI.Data Merge(List<TarkovAPI.Data> parts)
        {
            TarkovAPI.Data result = new TarkovAPI.Data();
            Dictionary<string, TarkovAPI.Item> byId = null;
            foreach (TarkovAPI.Data part in parts)
            {
                if (part == null)
                {
                    continue;
                }
                if (part.items != null)
                {
                    if (result.items == null)
                    {
                        result.items = part.items;
                        byId = new Dictionary<string, TarkovAPI.Item>();
                        foreach (TarkovAPI.Item item in part.items)
                        {
                            if (item != null && item.id != null && !byId.ContainsKey(item.id))
                            {
                                byId.Add(item.id, item);
                            }
                        }
                    }
                    else
                    {
                        foreach (TarkovAPI.Item item in part.items)
                        {
                            TarkovAPI.Item target;
                            if (item == null || item.id == null || !byId.TryGetValue(item.id, out target))
                            {
                                continue;
                            }
                            if (item.properties != null) target.properties = item.properties;
                            if (item.sellFor != null) target.sellFor = item.sellFor;
                            if (item.buyFor != null) target.buyFor = item.buyFor;
                            if (item.bartersUsing != null) target.bartersUsing = item.bartersUsing;
                            if (item.bartersFor != null) target.bartersFor = item.bartersFor;
                            if (item.craftsFor != null) target.craftsFor = item.craftsFor;
                            if (item.craftsUsing != null) target.craftsUsing = item.craftsUsing;
                            if (item.usedInTasks != null) target.usedInTasks = item.usedInTasks;
                        }
                    }
                }
                if (part.hideoutStations != null)
                {
                    result.hideoutStations = part.hideoutStations;
                }
            }
            return result;
        }

        public static string ExtractServerErrors(string responseContent)
        {
            try
            {
                ResponseShell shell = JsonConvert.DeserializeObject<ResponseShell>(responseContent);
                if (shell != null && shell.errors != null && shell.errors.Count > 0)
                {
                    List<string> messages = new List<string>();
                    foreach (ResponseShell.GQLError error in shell.errors)
                    {
                        if (error != null && !String.IsNullOrEmpty(error.message))
                        {
                            messages.Add(error.message);
                        }
                    }
                    if (messages.Count > 0)
                    {
                        return ": " + String.Join("; ", messages);
                    }
                }
            }
            catch
            {
                // response was not JSON in the expected shape; ignore
            }
            return "";
        }
    }
}
