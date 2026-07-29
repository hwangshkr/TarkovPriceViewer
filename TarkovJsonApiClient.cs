using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace TarkovPriceViewer
{
    // Fetches data from the json.tarkov.dev API (simple GET endpoints).
    //
    // Why: the GraphQL API (api.tarkov.dev/graphql) has been returning
    // HTTP 503 "GraphQL server unavailable" continuously since 2026-07-21
    // (see the-hideout/tarkov-api issue #474). The tarkov.dev website itself
    // runs on json.tarkov.dev, which the maintainers recommend as the live
    // replacement. This client downloads the JSON endpoints and converts them
    // into the same TarkovAPI.Data structure the rest of the app expects.
    public static class TarkovJsonApiClient
    {
        public const string BaseUrl = "https://json.tarkov.dev/";
        public const string FleaMarketName = "Flea Market"; // Overlay matches this exact string

        public static async Task<TarkovAPI.Data> FetchAll(string language, string gameMode)
        {
            if (String.IsNullOrEmpty(gameMode))
            {
                gameMode = "regular";
            }
            if (String.IsNullOrEmpty(language))
            {
                language = "en";
            }
            language = language.ToLowerInvariant();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(120);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "TarkovPriceViewer");

                JObject items = await GetTranslated(client, gameMode + "/items", language);
                JObject tasks = await GetTranslated(client, gameMode + "/tasks", language);
                JObject hideout = await GetTranslated(client, gameMode + "/hideout", language);
                JObject traders = await GetTranslated(client, gameMode + "/traders", language);
                JObject maps = await GetTranslated(client, gameMode + "/maps", language);
                JObject barters = await GetJson(client, gameMode + "/barters");
                JObject crafts = await GetJson(client, gameMode + "/crafts");

                TarkovAPI.Data result = Transform(
                    items["data"], tasks["data"], hideout["data"],
                    traders["data"], maps["data"], barters["data"], crafts["data"]);
                if (result == null || result.items == null || result.items.Count == 0)
                {
                    throw new Exception("json.tarkov.dev responses contained no item data");
                }
                return result;
            }
        }

        // ---------- HTTP ----------

        private static async Task<JObject> GetJson(HttpClient client, string path)
        {
            const int maxAttempts = 3;
            Exception lastError = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Debug.WriteLine("--> GET " + BaseUrl + path);
                    HttpResponseMessage response = await client.GetAsync(BaseUrl + path);
                    string content = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("json.tarkov.dev returned HTTP "
                            + (int)response.StatusCode + " " + response.StatusCode + " for /" + path);
                    }
                    return JObject.Parse(content);
                }
                catch (Exception ex)
                {
                    lastError = ex;
                    Debug.WriteLine("--> /" + path + " attempt " + attempt + "/" + maxAttempts + " failed: " + ex.Message);
                    if (attempt < maxAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3 * attempt));
                    }
                }
            }
            throw lastError;
        }

        // Fetches an endpoint plus its translation maps and applies them.
        private static async Task<JObject> GetTranslated(HttpClient client, string path, string language)
        {
            JObject root = await GetJson(client, path);
            JArray translations = root["translations"] as JArray;
            if (translations != null && translations.Count > 0)
            {
                JObject langMap = await TryGetLangMap(client, path + "_" + language);
                JObject enMap = null;
                if (language != "en")
                {
                    enMap = await TryGetLangMap(client, path + "_en");
                }
                ApplyTranslations(root, langMap, enMap);
            }
            return root;
        }

        private static async Task<JObject> TryGetLangMap(HttpClient client, string path)
        {
            try
            {
                JObject root = await GetJson(client, path);
                return root["data"] as JObject;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("--> language file /" + path + " unavailable: " + ex.Message);
                return null;
            }
        }

        // ---------- Translations ----------

        // The base endpoints contain translation keys; the "<path>_<lang>"
        // endpoints contain key->string maps. The response's "translations"
        // array holds JSONPath expressions pointing at every key that needs
        // replacing (same mechanism the tarkov.dev website uses).
        public static void ApplyTranslations(JObject root, JObject langMap, JObject enMap)
        {
            JArray translations = root["translations"] as JArray;
            if (translations == null || (langMap == null && enMap == null))
            {
                return;
            }
            foreach (JToken pathToken in translations)
            {
                string jPath = (string)pathToken;
                if (String.IsNullOrEmpty(jPath))
                {
                    continue;
                }
                try
                {
                    List<JToken> found = new List<JToken>(root.SelectTokens(jPath, false));
                    if (found.Count == 0 && jPath.Contains("[*]"))
                    {
                        // jsonpath-plus allows [*] over objects; Newtonsoft prefers .*
                        found = new List<JToken>(root.SelectTokens(jPath.Replace("[*]", ".*"), false));
                    }
                    foreach (JToken token in found)
                    {
                        if (token.Type != JTokenType.String)
                        {
                            continue;
                        }
                        string key = (string)token;
                        JToken translated = null;
                        if (langMap != null)
                        {
                            translated = langMap[key];
                        }
                        if (translated == null && enMap != null)
                        {
                            translated = enMap[key];
                        }
                        if (translated != null && translated.Type == JTokenType.String)
                        {
                            token.Replace(translated.DeepClone());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("--> translation path '" + jPath + "' failed: " + ex.Message);
                }
            }
        }

        // ---------- Transformation ----------

        public static TarkovAPI.Data Transform(JToken itemsData, JToken tasksData, JToken hideoutData,
            JToken tradersData, JToken mapsData, JToken bartersData, JToken craftsData)
        {
            TarkovAPI.Data result = new TarkovAPI.Data();
            result.items = new List<TarkovAPI.Item>();
            result.hideoutStations = new List<TarkovAPI.HideoutStation>();

            // Flea market info
            JToken flea = itemsData == null ? null : itemsData["fleaMarket"];
            double ti = Dbl(flea == null ? null : flea["sellOfferFeeRate"]) ?? 0.03;
            double tr = Dbl(flea == null ? null : flea["sellRequirementFeeRate"]) ?? 0.03;
            bool fleaEnabled = Bool(flea == null ? null : flea["enabled"]) ?? true;

            // Trader id -> name
            Dictionary<string, string> traderNames = new Dictionary<string, string>();
            foreach (JToken t in Values(tradersData))
            {
                string id = Str(t["id"]);
                if (id != null)
                {
                    traderNames[id] = Str(t["name"]);
                }
            }

            // Map id -> name
            Dictionary<string, string> mapNames = new Dictionary<string, string>();
            foreach (JToken m in Values(mapsData == null ? null : mapsData["maps"]))
            {
                string id = Str(m["id"]);
                if (id != null)
                {
                    mapNames[id] = Str(m["name"]);
                }
            }

            // ----- Items -----
            Dictionary<string, TarkovAPI.Item> byId = new Dictionary<string, TarkovAPI.Item>();
            foreach (JToken j in Values(itemsData == null ? null : itemsData["items"]))
            {
                TarkovAPI.Item item = new TarkovAPI.Item();
                item.id = Str(j["id"]);
                item.name = Str(j["name"]);
                item.normalizedName = Str(j["normalizedName"]);
                item.types = new List<string>();
                foreach (JToken t in (j["types"] as JArray) ?? new JArray())
                {
                    item.types.Add((string)t);
                }
                item.lastLowPrice = Int(j["lastLowPrice"]);
                item.avg24hPrice = Int(j["avg24hPrice"]);
                item.updated = Date(j["updated"]);
                item.wikiLink = Str(j["wikiLink"]);
                item.link = Str(j["link"]);
                if (item.link == null && item.normalizedName != null)
                {
                    item.link = "https://tarkov.dev/item/" + item.normalizedName;
                }
                item.width = Int(j["width"]);
                item.height = Int(j["height"]);

                JToken props = j["properties"];
                if (props != null && props.Type == JTokenType.Object)
                {
                    item.properties = new TarkovAPI.Properties();
                    item.properties._class = Int(props["class"]);
                    item.properties.defaultWidth = Int(props["defaultWidth"]);
                    item.properties.defaultHeight = Int(props["defaultHeight"]);
                }

                // Flea market fee for selling at lastLowPrice
                int? basePrice = Int(j["basePrice"]);
                if (fleaEnabled && basePrice != null && basePrice > 0
                    && item.lastLowPrice != null && item.lastLowPrice > 0)
                {
                    item.fleaMarketFee = FleaFee(basePrice.Value, item.lastLowPrice.Value, ti, tr);
                }

                // sellFor: flea (if it has a flea price) + traders
                item.sellFor = new List<TarkovAPI.SellFor>();
                if (fleaEnabled && item.lastLowPrice != null && item.lastLowPrice > 0)
                {
                    TarkovAPI.SellFor fleaSell = new TarkovAPI.SellFor();
                    fleaSell.currency = "RUB";
                    fleaSell.priceRUB = item.lastLowPrice;
                    fleaSell.vendor = new TarkovAPI.Vendor();
                    fleaSell.vendor.name = FleaMarketName;
                    item.sellFor.Add(fleaSell);
                }
                foreach (JToken offer in (j["sellToTrader"] as JArray) ?? new JArray())
                {
                    TarkovAPI.SellFor sf = new TarkovAPI.SellFor();
                    sf.currency = Str(offer["currency"]);
                    sf.priceRUB = Int(offer["priceRUB"]);
                    sf.vendor = new TarkovAPI.Vendor();
                    sf.vendor.name = LookUp(traderNames, Str(offer["trader"]));
                    item.sellFor.Add(sf);
                }

                // buyFor: flea (avg24h, falls back to lastLow) + traders
                item.buyFor = new List<TarkovAPI.BuyFor>();
                int? fleaBuy = item.avg24hPrice ?? item.lastLowPrice;
                if (fleaEnabled && fleaBuy != null && fleaBuy > 0)
                {
                    TarkovAPI.BuyFor fleaBuyFor = new TarkovAPI.BuyFor();
                    fleaBuyFor.currency = "RUB";
                    fleaBuyFor.priceRUB = fleaBuy;
                    fleaBuyFor.vendor = new TarkovAPI.Vendor();
                    fleaBuyFor.vendor.name = FleaMarketName;
                    item.buyFor.Add(fleaBuyFor);
                }
                foreach (JToken offer in (j["buyFromTrader"] as JArray) ?? new JArray())
                {
                    TarkovAPI.BuyFor bf = new TarkovAPI.BuyFor();
                    bf.currency = Str(offer["currency"]);
                    bf.priceRUB = Int(offer["priceRUB"]);
                    bf.vendor = new TarkovAPI.Vendor();
                    bf.vendor.name = LookUp(traderNames, Str(offer["trader"]));
                    bf.vendor.minTraderLevel = Int(offer["minTraderLevel"]);
                    item.buyFor.Add(bf);
                }

                item.bartersUsing = new List<TarkovAPI.BartersUsing>();
                item.bartersFor = new List<TarkovAPI.BartersFor>();
                item.craftsFor = new List<TarkovAPI.CraftsFor>();
                item.craftsUsing = new List<TarkovAPI.CraftsUsing>();
                item.usedInTasks = new List<TarkovAPI.UsedInTask>();

                if (item.id != null && !byId.ContainsKey(item.id))
                {
                    byId.Add(item.id, item);
                    result.items.Add(item);
                }
            }

            // ----- Tasks -----
            Dictionary<string, string> taskNames = new Dictionary<string, string>();
            foreach (JToken t in Values(tasksData == null ? null : tasksData["tasks"]))
            {
                string taskId = Str(t["id"]);
                TarkovAPI.UsedInTask uit = new TarkovAPI.UsedInTask();
                uit.id = taskId;
                uit.name = Str(t["name"]);
                uit.trader = new TarkovAPI.Trader();
                uit.trader.name = LookUp(traderNames, Str(t["trader"]));
                string taskMap = LookUp(mapNames, Str(t["map"]));
                if (taskMap != null)
                {
                    uit.map = new TarkovAPI.Map();
                    uit.map.name = taskMap;
                }
                uit.minPlayerLevel = Int(t["minPlayerLevel"]);
                uit.traderLevelRequirements = new List<TarkovAPI.TraderLevelRequirement>();
                foreach (JToken req in (t["traderRequirements"] as JArray) ?? new JArray())
                {
                    TarkovAPI.TraderLevelRequirement tlr = new TarkovAPI.TraderLevelRequirement();
                    tlr.level = Int(req["level"]) ?? Int(req["value"]);
                    uit.traderLevelRequirements.Add(tlr);
                }
                if (taskId != null)
                {
                    taskNames[taskId] = uit.name;
                }

                // Objectives are kept per linked item, filtered down to that
                // item only. Embedding the full task (all objectives with all
                // eligible items) into every linked item makes the object
                // graph explode: quests accepting "any of N items" would be
                // copied N times with N-item lists each, and serializing the
                // cache then produces a multi-gigabyte JSON that crashes
                // Newtonsoft's StringBuilder. The overlay only ever calls
                // GetItemCount(item.id), which needs just the objectives that
                // mention the item in question.
                uit.objectives = new List<TarkovAPI.Objective>();
                Dictionary<string, List<TarkovAPI.Objective>> objectivesByItem
                    = new Dictionary<string, List<TarkovAPI.Objective>>();
                foreach (JToken obj in (t["objectives"] as JArray) ?? new JArray())
                {
                    string objId = Str(obj["id"]);
                    bool? optional = Bool(obj["optional"]);
                    string objType = Str(obj["type"]);
                    int? count = Int(obj["count"]);
                    foreach (JToken itemId in (obj["items"] as JArray) ?? new JArray())
                    {
                        string iid = Str(itemId);
                        if (iid == null)
                        {
                            continue;
                        }
                        TarkovAPI.Objective o = new TarkovAPI.Objective();
                        o.id = objId;
                        o.optional = optional;
                        o.type = objType;
                        o.count = count;
                        o.items = new List<TarkovAPI.TaskItem>();
                        TarkovAPI.TaskItem ti2 = new TarkovAPI.TaskItem();
                        ti2.id = iid;
                        TarkovAPI.Item known;
                        ti2.name = byId.TryGetValue(iid, out known) ? known.name : null;
                        o.items.Add(ti2);
                        List<TarkovAPI.Objective> perItem;
                        if (!objectivesByItem.TryGetValue(iid, out perItem))
                        {
                            perItem = new List<TarkovAPI.Objective>();
                            objectivesByItem.Add(iid, perItem);
                        }
                        perItem.Add(o);
                    }
                }
                foreach (KeyValuePair<string, List<TarkovAPI.Objective>> pair in objectivesByItem)
                {
                    TarkovAPI.Item target;
                    if (!byId.TryGetValue(pair.Key, out target))
                    {
                        continue;
                    }
                    TarkovAPI.UsedInTask slim = new TarkovAPI.UsedInTask();
                    slim.id = uit.id;
                    slim.name = uit.name;
                    slim.trader = uit.trader;
                    slim.map = uit.map;
                    slim.minPlayerLevel = uit.minPlayerLevel;
                    slim.traderLevelRequirements = uit.traderLevelRequirements;
                    slim.objectives = pair.Value;
                    target.usedInTasks.Add(slim);
                }
            }

            // ----- Hideout stations -----
            Dictionary<string, string> stationNames = new Dictionary<string, string>();
            foreach (JToken s in Values(hideoutData))
            {
                string sid = Str(s["id"]);
                TarkovAPI.HideoutStation station = new TarkovAPI.HideoutStation();
                station.name = Str(s["name"]);
                if (sid != null)
                {
                    stationNames[sid] = station.name;
                }
                station.levels = new List<TarkovAPI.Level>();
                foreach (JToken lvl in (s["levels"] as JArray) ?? new JArray())
                {
                    TarkovAPI.Level level = new TarkovAPI.Level();
                    level.id = Str(lvl["id"]);
                    level.level = Int(lvl["level"]);
                    level.itemRequirements = new List<TarkovAPI.ItemRequirement>();
                    foreach (JToken req in (lvl["itemRequirements"] as JArray) ?? new JArray())
                    {
                        TarkovAPI.ItemRequirement ir = new TarkovAPI.ItemRequirement();
                        ir.count = Int(req["count"]);
                        string iid = Str(req["item"]);
                        ir.item = new TarkovAPI.HideoutItem();
                        ir.item.id = iid;
                        TarkovAPI.Item known;
                        ir.item.name = (iid != null && byId.TryGetValue(iid, out known)) ? known.name : null;
                        ir.attributes = AttributesList(req["attributes"]);
                        level.itemRequirements.Add(ir);
                    }
                    station.levels.Add(level);
                }
                result.hideoutStations.Add(station);
            }

            // ----- Barters -----
            foreach (JToken b in Values(bartersData))
            {
                TarkovAPI.Trader trader = new TarkovAPI.Trader();
                trader.name = LookUp(traderNames, Str(b["trader"]));
                trader.levels = new List<TarkovAPI.Level>();
                TarkovAPI.Level tl = new TarkovAPI.Level();
                tl.level = Int(b["minTraderLevel"]) ?? Int(b["level"]) ?? 1;
                trader.levels.Add(tl);

                List<TarkovAPI.RequiredItem> reqs = RequiredItems(b["requiredItems"], byId);
                List<TarkovAPI.RewardItem> rewards = RewardItems(RewardsToken(b), byId);

                TarkovAPI.BartersFor bFor = new TarkovAPI.BartersFor();
                bFor.trader = trader;
                bFor.requiredItems = reqs;
                bFor.rewardItems = rewards;
                string unlockName = LookUp(taskNames, Str(b["taskUnlock"]));
                if (unlockName != null)
                {
                    bFor.taskUnlock = new TarkovAPI.TaskUnlock();
                    bFor.taskUnlock.name = unlockName;
                }

                TarkovAPI.BartersUsing bUsing = new TarkovAPI.BartersUsing();
                bUsing.trader = trader;
                bUsing.requiredItems = reqs;
                bUsing.rewardItems = rewards;

                AddToItems(rewards, byId, delegate (TarkovAPI.Item it) { it.bartersFor.Add(bFor); });
                AddRequiredToItems(reqs, byId, delegate (TarkovAPI.Item it) { it.bartersUsing.Add(bUsing); });
            }

            // ----- Crafts -----
            foreach (JToken c in Values(craftsData))
            {
                TarkovAPI.Station station = new TarkovAPI.Station();
                station.name = LookUp(stationNames, Str(c["station"]));
                station.levels = new List<TarkovAPI.Level>();
                TarkovAPI.Level sl = new TarkovAPI.Level();
                sl.level = Int(c["level"]) ?? 1;
                station.levels.Add(sl);

                List<TarkovAPI.RequiredItem> reqs = RequiredItems(c["requiredItems"], byId);
                List<TarkovAPI.RewardItem> rewards = RewardItems(RewardsToken(c), byId);

                TarkovAPI.CraftsFor cFor = new TarkovAPI.CraftsFor();
                cFor.station = station;
                cFor.requiredItems = reqs;
                cFor.rewardItems = rewards;

                TarkovAPI.CraftsUsing cUsing = new TarkovAPI.CraftsUsing();
                cUsing.station = station;
                cUsing.requiredItems = reqs;
                cUsing.rewardItems = rewards;

                AddToItems(rewards, byId, delegate (TarkovAPI.Item it) { it.craftsFor.Add(cFor); });
                AddRequiredToItems(reqs, byId, delegate (TarkovAPI.Item it) { it.craftsUsing.Add(cUsing); });
            }

            return result;
        }

        // ---------- Small helpers ----------

        // rewardItems, or the single offeredItem/productItem wrapped as a list
        private static JToken RewardsToken(JToken tradeOrCraft)
        {
            JToken rewards = tradeOrCraft["rewardItems"];
            if (rewards != null && rewards.Type == JTokenType.Array)
            {
                return rewards;
            }
            JToken single = tradeOrCraft["offeredItem"];
            if (single == null || single.Type == JTokenType.Null)
            {
                single = tradeOrCraft["productItem"];
            }
            if (single == null || single.Type == JTokenType.Null)
            {
                return new JArray();
            }
            JArray wrapped = new JArray();
            wrapped.Add(single);
            return wrapped;
        }

        private static List<TarkovAPI.RequiredItem> RequiredItems(JToken arr, Dictionary<string, TarkovAPI.Item> byId)
        {
            List<TarkovAPI.RequiredItem> list = new List<TarkovAPI.RequiredItem>();
            foreach (JToken req in (arr as JArray) ?? new JArray())
            {
                TarkovAPI.RequiredItem ri = new TarkovAPI.RequiredItem();
                ri.item = StubItem(Str(req["item"]), byId);
                ri.count = Flt(req["count"]) ?? Flt(req["quantity"]) ?? 1;
                ri.quantity = ri.count;
                list.Add(ri);
            }
            return list;
        }

        private static List<TarkovAPI.RewardItem> RewardItems(JToken arr, Dictionary<string, TarkovAPI.Item> byId)
        {
            List<TarkovAPI.RewardItem> list = new List<TarkovAPI.RewardItem>();
            foreach (JToken rew in (arr as JArray) ?? new JArray())
            {
                TarkovAPI.RewardItem ri = new TarkovAPI.RewardItem();
                ri.item = StubItem(Str(rew["item"]), byId);
                ri.count = Int(rew["count"]) ?? 1;
                ri.quantity = Flt(rew["count"]) ?? 1;
                list.Add(ri);
            }
            return list;
        }

        // A flat item reference ({id, name} only) to avoid reference cycles
        // when the merged data is serialized to the local cache file.
        private static TarkovAPI.Item StubItem(string id, Dictionary<string, TarkovAPI.Item> byId)
        {
            TarkovAPI.Item stub = new TarkovAPI.Item();
            stub.id = id;
            TarkovAPI.Item known;
            if (id != null && byId.TryGetValue(id, out known))
            {
                stub.name = known.name;
            }
            return stub;
        }

        private static void AddToItems(List<TarkovAPI.RewardItem> rewards,
            Dictionary<string, TarkovAPI.Item> byId, Action<TarkovAPI.Item> action)
        {
            HashSet<string> done = new HashSet<string>();
            foreach (TarkovAPI.RewardItem rew in rewards)
            {
                string id = rew.item == null ? null : rew.item.id;
                TarkovAPI.Item target;
                if (id != null && done.Add(id) && byId.TryGetValue(id, out target))
                {
                    action(target);
                }
            }
        }

        private static void AddRequiredToItems(List<TarkovAPI.RequiredItem> reqs,
            Dictionary<string, TarkovAPI.Item> byId, Action<TarkovAPI.Item> action)
        {
            HashSet<string> done = new HashSet<string>();
            foreach (TarkovAPI.RequiredItem req in reqs)
            {
                string id = req.item == null ? null : req.item.id;
                TarkovAPI.Item target;
                if (id != null && done.Add(id) && byId.TryGetValue(id, out target))
                {
                    action(target);
                }
            }
        }

        private static List<TarkovAPI.HideoutAttributes> AttributesList(JToken attributes)
        {
            List<TarkovAPI.HideoutAttributes> list = new List<TarkovAPI.HideoutAttributes>();
            JObject obj = attributes as JObject;
            if (obj != null)
            {
                foreach (JProperty p in obj.Properties())
                {
                    TarkovAPI.HideoutAttributes a = new TarkovAPI.HideoutAttributes();
                    a.type = p.Name;
                    if (p.Value.Type == JTokenType.Boolean)
                    {
                        a.value = ((bool)p.Value) ? "true" : "false";
                    }
                    else
                    {
                        a.value = Str(p.Value);
                    }
                    list.Add(a);
                }
            }
            return list;
        }

        // Flea market listing fee, same formula as tarkov.dev uses
        // (https://escapefromtarkov.fandom.com/wiki/Trading#Flea_Market)
        public static int FleaFee(double basePrice, double sellPrice, double ti, double tr)
        {
            double v0 = basePrice;
            double vr = sellPrice;
            double p0 = Math.Log10(v0 / vr);
            double pr = Math.Log10(vr / v0);
            if (vr < v0)
            {
                p0 = Math.Pow(p0, 1.08);
            }
            else
            {
                pr = Math.Pow(pr, 1.08);
            }
            return (int)Math.Ceiling(v0 * ti * Math.Pow(4, p0) + vr * tr * Math.Pow(4, pr));
        }

        // Enumerates values of either a JSON object-map or an array.
        private static IEnumerable<JToken> Values(JToken token)
        {
            if (token == null)
            {
                yield break;
            }
            JObject obj = token as JObject;
            if (obj != null)
            {
                foreach (JProperty p in obj.Properties())
                {
                    yield return p.Value;
                }
                yield break;
            }
            JArray arr = token as JArray;
            if (arr != null)
            {
                foreach (JToken t in arr)
                {
                    yield return t;
                }
            }
        }

        private static string LookUp(Dictionary<string, string> dict, string key)
        {
            string value;
            if (key != null && dict.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        private static string Str(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null || t.Type == JTokenType.Undefined)
            {
                return null;
            }
            if (t.Type == JTokenType.String)
            {
                return (string)t;
            }
            if (t.Type == JTokenType.Object || t.Type == JTokenType.Array)
            {
                return null;
            }
            return t.ToString();
        }

        private static int? Int(JToken t)
        {
            double? d = Dbl(t);
            if (d == null)
            {
                return null;
            }
            return (int)Math.Round(d.Value);
        }

        private static float? Flt(JToken t)
        {
            double? d = Dbl(t);
            if (d == null)
            {
                return null;
            }
            return (float)d.Value;
        }

        private static double? Dbl(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null || t.Type == JTokenType.Undefined)
            {
                return null;
            }
            if (t.Type == JTokenType.Integer || t.Type == JTokenType.Float)
            {
                return (double)t;
            }
            double parsed;
            if (t.Type == JTokenType.String
                && double.TryParse((string)t, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }
            return null;
        }

        private static bool? Bool(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null || t.Type == JTokenType.Undefined)
            {
                return null;
            }
            if (t.Type == JTokenType.Boolean)
            {
                return (bool)t;
            }
            return null;
        }

        private static DateTime? Date(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null || t.Type == JTokenType.Undefined)
            {
                return null;
            }
            if (t.Type == JTokenType.Date)
            {
                return (DateTime)t;
            }
            DateTime parsed;
            if (t.Type == JTokenType.String
                && DateTime.TryParse((string)t, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.AdjustToUniversal, out parsed))
            {
                return parsed;
            }
            double? unix = Dbl(t);
            if (unix != null && unix > 0)
            {
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                // handle both seconds and milliseconds
                return unix > 100000000000d ? epoch.AddMilliseconds(unix.Value) : epoch.AddSeconds(unix.Value);
            }
            return null;
        }
    }
}
