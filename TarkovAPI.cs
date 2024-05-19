using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TarkovPriceViewer
{
    public class TarkovAPI
    {
        public class BartersFor
        {
            public Trader trader { get; set; }
            public List<RequiredItem> requiredItems { get; set; }
            public List<RewardItem> rewardItems { get; set; }
            public TaskUnlock taskUnlock { get; set; }
        }

        public class BartersUsing
        {
            public Trader trader { get; set; }
            public List<RequiredItem> requiredItems { get; set; }
            public List<RewardItem> rewardItems { get; set; }
        }

        public class BuyFor
        {
            public string currency { get; set; }
            public int? priceRUB { get; set; }
            public Vendor vendor { get; set; }
        }

        public class CraftsFor
        {
            public Station station { get; set; }
            public List<RequiredItem> requiredItems { get; set; }
            public List<RewardItem> rewardItems { get; set; }
        }

        public class CraftsUsing
        {
            public Station station { get; set; }
            public List<RequiredItem> requiredItems { get; set; }
            public List<RewardItem> rewardItems { get; set; }
        }

        public class Data
        {
            public List<Item> items { get; set; }
            public List<HideoutStation> hideoutStations { get; set; }
        }

        public class DefaultAmmo
        {
            public string name { get; set; }
        }

        public class HideoutStation
        {
            public string name { get; set; }
            public List<Level> levels { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string name { get; set; }
            public string normalizedName { get; set; }
            public List<string> types { get; set; }
            public int? lastLowPrice { get; set; }
            public int? avg24hPrice { get; set; }
            public DateTime? updated { get; set; }
            public int? fleaMarketFee { get; set; }
            public string link { get; set; }
            public string wikiLink { get; set; }
            public int? width { get; set; }
            public int? height { get; set; }
            public Properties properties { get; set; }
            public List<SellFor> sellFor { get; set; }
            public List<BuyFor> buyFor { get; set; }
            public List<BartersUsing> bartersUsing { get; set; }
            public List<CraftsFor> craftsFor { get; set; }
            public List<CraftsUsing> craftsUsing { get; set; }
            public List<BartersFor> bartersFor { get; set; }
            public List<UsedInTask> usedInTasks { get; set; }

            public Ballistic ballistic = null;
            public string lootTier = null;
            public string className = null;
            public String[] Data()
            {
                return (new String[] {
                    name
                    , ""
                    , ""
                    , ""
                    , ""
                    , ""
                    , ""
                });
                //return (new String[] {
                //    name
                //    , (recoil != null ? recoil : "")
                //    , (accuracy != null ? accuracy : "")
                //    , (ergo != null ? ergo : "")
                //    , (price_last != null ? price_last : "")
                //    , (buy_from_trader_price != null ? buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈") : "")
                //    , (buy_from_trader != null ? buy_from_trader : "")
                //});
            }
        }

        public class ItemRequirement
        {
            public Item item { get; set; }
            public int? count { get; set; }
        }

        public class Level
        {
            public int? level { get; set; }
            public string id { get; set; }
            public List<ItemRequirement> itemRequirements { get; set; }
        }

        public class Map
        {
            public string name { get; set; }
        }

        public class Objective
        {
            public string id { get; set; }
            public string description { get; set; }
            public List<Map> maps { get; set; }
        }

        public class Properties
        {
            //public string caliber { get; set; }
            //public int? ergonomics { get; set; }
            //public int? defaultRecoilVertical { get; set; }
            //public int? defaultRecoilHorizontal { get; set; }
            public int? defaultWidth { get; set; }
            public int? defaultHeight { get; set; }
            //public DefaultAmmo defaultAmmo { get; set; }
            //public int? uses { get; set; }
            //public int? accuracyModifier { get; set; }

            [JsonProperty(PropertyName = "class")]
            public int? _class { get; set; }

            //public int? damage { get; set; }
            //public int? projectileCount { get; set; }
            //public int? penetrationPower { get; set; }
            //public int? armorDamage { get; set; }
            //public double? fragmentationChance { get; set; }
            //public string ammoType { get; set; }
        }

        public class RequiredItem
        {
            public Item item { get; set; }
            public float? count { get; set; }
            public float? quantity { get; set; }
        }

        public class RewardItem
        {
            public Item item { get; set; }
            public float? count { get; set; }
            public float? quantity { get; set; }
        }

        public class Root
        {
            public Data data { get; set; }
        }

        public class SellFor
        {
            public string currency { get; set; }
            public int? priceRUB { get; set; }
            public Vendor vendor { get; set; }
        }

        public class Station
        {
            public string name { get; set; }
            public List<Level> levels { get; set; }
        }

        public class TaskUnlock
        {
            public string name { get; set; }
        }

        public class Trader
        {
            public string name { get; set; }
            public List<Level> levels { get; set; }
        }

        public class TraderLevelRequirement
        {
            public int? level { get; set; }
        }

        public class UsedInTask
        {
            public List<Objective> objectives { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public Trader trader { get; set; }
            public Map map { get; set; }
            public int? minPlayerLevel { get; set; }
            public List<TraderLevelRequirement> traderLevelRequirements { get; set; }
        }

        public class Vendor
        {
            public string name { get; set; }
            public int? minTraderLevel { get; set; }
        }
    }
}

/*

Item:
public Ballistic ballistic = null;
public string lootTier = null;
public string className = null;

 
public class Properties
{
    //public string caliber { get; set; }
    //public int? ergonomics { get; set; }
    //public int? defaultRecoilVertical { get; set; }
    //public int? defaultRecoilHorizontal { get; set; }
    public int? defaultWidth { get; set; }
    public int? defaultHeight { get; set; }
    //public DefaultAmmo defaultAmmo { get; set; }
    //public int? uses { get; set; }
    //public int? accuracyModifier { get; set; }

    [JsonProperty(PropertyName = "class")]
    public int? _class { get; set; }

    //public int? damage { get; set; }
    //public int? projectileCount { get; set; }
    //public int? penetrationPower { get; set; }
    //public int? armorDamage { get; set; }
    //public double? fragmentationChance { get; set; }
    //public string ammoType { get; set; }
}
*/

