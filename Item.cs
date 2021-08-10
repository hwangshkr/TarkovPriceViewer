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
        public String name_address;
        public String price_last;
        public String price_day;
        public String price_week;
        public String trader_sell;
        public String trader_sell_price;
        public String trader_buy;
        public String trader_buy_price;
        public String Needs;
        public String last_updated;
        public DateTime last_fetch;
    }
}
