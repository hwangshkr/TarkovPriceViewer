using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TarkovPriceViewer
{
    public class TarkovAPI
    {
        public class BuyFor
        {
            public string currency { get; set; }
            public int? priceRUB { get; set; }
            public Vendor vendor { get; set; }
        }

        public class Data
        {
            public List<Item> items { get; set; }
        }

        public class DefaultAmmo
        {
            public string name { get; set; }
        }

        public class Item
        {
            public string name { get; set; }
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
            public List<UsedInTask> usedInTasks { get; set; }

            public Ballistic ballistic = null;
            public string lootTier = null;
            public string className = null;
        }

        public class Map
        {
            public string name { get; set; }
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

        public class Trader
        {
            public string name { get; set; }
        }

        public class TraderLevelRequirement
        {
            public int? level { get; set; }
        }

        public class UsedInTask
        {
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
public Ballistic ballistic = null;
public string lootTier = null;
*/