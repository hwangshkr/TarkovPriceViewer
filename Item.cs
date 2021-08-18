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
        public String price_day;
        public String price_week;
        public String sell_to_trader;
        public String sell_to_trader_price;
        public String buy_from_trader;
        public String buy_from_trader_price;
        public String Needs;
        public String Crafts;
        public String last_updated;
        public DateTime last_fetch;
    }
}
