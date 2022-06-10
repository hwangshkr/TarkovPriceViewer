using System;

namespace TarkovPriceViewer
{
    public class Item
    {
        public String name_display;
        public char[] name_compare;
        public String name_display2;
        public char[] name_compare2;
        public bool isname2 = false;
        public String market_address;
        public String wiki_address;
        public String price_last;
        public String fee;
        public String price_day;
        public String price_week;
        public String sell_to_trader;
        public String sell_to_trader_price;
        public String buy_from_trader;
        public String buy_from_trader_price;
        public String last_updated;

        public String needs;
        public String bartersandcrafts;
        public String type;
        public String recoil;
        public String accuracy;
        public String ergo;

        public DateTime last_fetch;
        public Ballistic ballistic = null;

        public String[] Data()
        {
            return (new String[] {
                (isname2 ? name_display2 : name_display)
                , (recoil != null ? recoil : "")
                , (accuracy != null ? accuracy : "")
                , (ergo != null ? ergo : "")
                , (price_last != null ? price_last : "")
                , (buy_from_trader_price != null ? buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈") : "")
                , (buy_from_trader != null ? buy_from_trader : "")
            });
        }
    }
}
