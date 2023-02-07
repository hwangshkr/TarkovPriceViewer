using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static TarkovPriceViewer.TarkovAPI;

namespace TarkovPriceViewer
{
    public partial class Overlay : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private static readonly int GWL_EXSTYLE = -20;
        private static readonly int WS_EX_TOOLWINDOW = 0x00000080;
        private static readonly int WS_EX_LAYERED = 0x80000;
        private static readonly int WS_EX_TRANSPARENT = 0x20;
        private static int compare_size = 0;
        private static bool isinfoform = true;
        private static bool ismoving = false;
        private static int x, y;
        public static DateTime startTime;
        private static string waitinfForTooltipText = Program.waitingForTooltip;
        private static int DotsCounter = 0;

        private Object _lock = new Object();

        public Overlay(bool _isinfoform)
        {
            InitializeComponent();
            isinfoform = _isinfoform;
            this.TopMost = true;
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            if (isinfoform)
            {
                this.Opacity = Int32.Parse(Program.settings["Overlay_Transparent"]) * 0.01;
                SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT);
            } else
            {
                SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TOOLWINDOW);
            }
            settingFormPos();
            initializeCompareData();
            initializeBallistics();
            iteminfo_panel.Visible = false;
            itemcompare_panel.Visible = false;
        }

        public void settingFormPos()
        {
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void initializeCompareData()
        {
            ItemCompareGrid.ColumnCount = 7;
            ItemCompareGrid.Columns[0].Name = "Name";
            ItemCompareGrid.Columns[1].Name = "Recoil";
            ItemCompareGrid.Columns[2].Name = "Accuracy";
            ItemCompareGrid.Columns[3].Name = "Ergo";
            ItemCompareGrid.Columns[4].Name = "Flea";
            ItemCompareGrid.Columns[5].Name = "NPC";
            ItemCompareGrid.Columns[6].Name = "LL";
            ItemCompareGrid.Visible = false;
            ItemCompareGrid.ClearSelection();
            ItemCompareGrid.SortCompare += new DataGridViewSortCompareEventHandler(ItemCompareGrid_SortCompare);
            ResizeGrid(ItemCompareGrid);
        }

        public void initializeBallistics()
        {
            iteminfo_ball.ColumnCount = 9;
            iteminfo_ball.Columns[0].Name = "Type";
            iteminfo_ball.Columns[1].Name = "Name";
            iteminfo_ball.Columns[2].Name = "Damage";
            iteminfo_ball.Columns[3].Name = "1";
            iteminfo_ball.Columns[4].Name = "2";
            iteminfo_ball.Columns[5].Name = "3";
            iteminfo_ball.Columns[6].Name = "4";
            iteminfo_ball.Columns[7].Name = "5";
            iteminfo_ball.Columns[8].Name = "6";
            iteminfo_ball.Visible = false;
            iteminfo_ball.ClearSelection();
            ResizeGrid(iteminfo_ball);
        }

        private void ItemCompareGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 0 || e.Column.Index == 5 || e.Column.Index == 6) return;
            int s1 = 0;
            Int32.TryParse(String.Join("", e.CellValue1.ToString().Replace(",", "").Split(Program.splitcur)), out s1);
            int s2 = 0;
            Int32.TryParse(String.Join("", e.CellValue2.ToString().Replace(",", "").Split(Program.splitcur)), out s2);
            e.SortResult = s1.CompareTo(s2);
            e.Handled = true;
        }

        public void ResizeGrid(DataGridView view)
        {
            view.ClientSize = new Size(view.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 10,
                view.Rows.GetRowsHeight(DataGridViewElementStates.None) + 22);
            view.Refresh();
        }

        public void SetBallisticsColor(Item item)
        {
            for (int b = 0; b < iteminfo_ball.Rows.Count; b++)
            {
                for (int i = 0; i < iteminfo_ball.Rows[b].Cells.Count; i++)
                {
                    if (i == 1)
                    {
                        if (iteminfo_ball.Rows[b].Cells[i].Value.Equals(item.name_display) || iteminfo_ball.Rows[b].Cells[i].Value.Equals(item.name_display2))
                        {
                            //iteminfo_ball.Rows.List).Items[6])).Cells.List).Items[1]
                            iteminfo_ball.Rows[b].Cells[i].Style.ForeColor = Color.Gold;
                            iteminfo_ball.Rows[b].Cells[i-1].Style.ForeColor = Color.Gold;
                            iteminfo_ball.Rows[b].Cells[i+1].Style.ForeColor = Color.Gold;
                        }
                    } else if (i >= 3)
                    {
                        try
                        {
                            int level = 0;
                            Int32.TryParse((String)iteminfo_ball.Rows[b].Cells[i].Value, out level);
                            iteminfo_ball.Rows[b].Cells[i].Style.BackColor = Program.BEColor[level];
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
            iteminfo_ball.Refresh();
        }

        public void SetBallisticsColorAPI(TarkovAPI.Item item)
        {
            for (int b = 0; b < iteminfo_ball.Rows.Count; b++)
            {
                for (int i = 0; i < iteminfo_ball.Rows[b].Cells.Count; i++)
                {
                    if (i == 1)
                    {
                        if (iteminfo_ball.Rows[b].Cells[i].Value.Equals(item.name))
                        {
                            iteminfo_ball.Rows[b].Cells[i].Style.ForeColor = Color.Gold;
                            iteminfo_ball.Rows[b].Cells[i - 1].Style.ForeColor = Color.Gold;
                            iteminfo_ball.Rows[b].Cells[i + 1].Style.ForeColor = Color.Gold;
                        }
                    }
                    else if (i >= 3)
                    {
                        try
                        {
                            int level = 0;
                            Int32.TryParse((String)iteminfo_ball.Rows[b].Cells[i].Value, out level);
                            iteminfo_ball.Rows[b].Cells[i].Style.BackColor = Program.BEColor[level];
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }
            }
            iteminfo_ball.Refresh();
        }

        public void ShowInfo(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        if (item == null || item.market_address == null)
                        {
                            if (MainForm.timer.Enabled || MainForm.WaitingForTooltip)
                                iteminfo_text.Text = waitinfForTooltipText;
                            else
                                iteminfo_text.Text = Program.notfound;
                        }
                        else if (item.last_updated == null && item.banned_from_flea == null)
                        {
                            iteminfo_text.Text = Program.noflea;
                        }
                        else if (item.banned_from_flea != null) //Handle items banned from Flea Market
                        {
                            int tp;
                            Int32.TryParse(String.Join("", item.sell_to_trader_price.Replace(",", "").Split(Program.splitcur)), out tp);

                            StringBuilder sb = new StringBuilder();
                            sb.Append(String.Format("Name : {0}\n", item.isname2 ? item.name_display2 : item.name_display));
                            sb.Append(String.Format("Sell to Trader : {0} --> Profit : {1}\n", item.sell_to_trader, item.sell_to_trader_price));

                            if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.buy_from_trader != null)
                            {
                                sb.Append(String.Format("\nBuy from Trader : {0} --> {1}\n", item.buy_from_trader, item.buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            if (Convert.ToBoolean(Program.settings["Needs"]) && item.needs != null)
                            {
                                sb.Append(String.Format("\nNeeds :\n{0}\n", item.needs));
                            }
                            if (Convert.ToBoolean(Program.settings["Barters_and_Crafts"]) && item.bartersandcrafts != null)
                            {
                                sb.Append(String.Format("\nBarters & Crafts :\n{0}\n", item.bartersandcrafts));
                            }
                            sb.Append(String.Format("Banned from Flea Market"));
                            iteminfo_ball.Rows.Clear();
                            iteminfo_text.Text = sb.ToString().Trim();
                            setTextColors(item);
                            if (item.ballistic != null)
                            {
                                foreach (Ballistic b in item.ballistic.Calibarlist)
                                {
                                    iteminfo_ball.Rows.Add(b.Data());
                                }
                                iteminfo_ball.Visible = true;
                                SetBallisticsColor(item);
                                ResizeGrid(iteminfo_ball);
                            }
                        }
                        else
                        {
                            int l, f, tp, btp = 0;
                            Int32.TryParse(String.Join("", item.price_last.Replace(",", "").Split(Program.splitcur)), out l);
                            Int32.TryParse(String.Join("", item.fee.Replace(",", "").Split(Program.splitcur)), out f);
                            Int32.TryParse(String.Join("", item.sell_to_trader_price.Replace(",", "").Split(Program.splitcur)), out tp);

                            if (item.buy_from_trader != null)
                            {
                                Int32.TryParse(String.Join("", item.buy_from_trader_price.Replace(",", "").Split(Program.splitcur)), out btp);
                            }
                            
                            int flea_profit = l - f;
                            char currency = item.price_last[item.price_last.Length - 1];

                            StringBuilder sb = new StringBuilder();
                            sb.Append(String.Format("Name : {0}\n", item.isname2 ? item.name_display2 : item.name_display));

                            if (tp >= flea_profit)
                            {
                                sb.Append(String.Format("Sell to Trader : {0} --> Profit : {1}\n", item.sell_to_trader, item.sell_to_trader_price));
                            }
                            else
                            {
                                sb.Append(String.Format("Sell to Flea Market --> Profit : {0}{1}\n", flea_profit.ToString("N0"), currency));
                            }

                            if (l >= btp && item.buy_from_trader != null)
                            {
                                sb.Append(String.Format("\nBetter to Buy from Trader : {0} --> {1}\n\n", item.buy_from_trader, item.buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            else
                            {
                                sb.Append(String.Format("\nBetter to Buy from Flea Market : {0}{1}\n\n", l.ToString("N0"), currency));
                            }

                            if (Convert.ToBoolean(Program.settings["Show_Last_Price"]))
                            {
                                sb.Append(String.Format("Last Price : {0}  ({1})\n", item.price_last, item.last_updated));
                            }
                            if (item.fee != null)
                            {
                                sb.Append(String.Format("Profit : {0}{1} (Fee : {2})\n", flea_profit.ToString("N0"), currency, item.fee));
                            }
                            if (Convert.ToBoolean(Program.settings["Show_Day_Price"]) && item.price_day != null)
                            {
                                sb.Append(String.Format("Day Price : {0}\n", item.price_day));
                            }
                            if (Convert.ToBoolean(Program.settings["Show_Week_Price"]) && item.price_week != null)
                            {
                                sb.Append(String.Format("Week Price : {0}\n", item.price_week));
                            }
                            if (Convert.ToBoolean(Program.settings["Sell_to_Trader"]) && item.sell_to_trader != null)
                            {
                                sb.Append(String.Format("\nSell to Trader : {0} --> {1}", item.sell_to_trader, item.sell_to_trader_price));
                            }
                            if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.buy_from_trader != null)
                            {
                                sb.Append(String.Format("\nBuy from Trader : {0} --> {1}", item.buy_from_trader, item.buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈")));
                            }
                            if (Convert.ToBoolean(Program.settings["Needs"]) && item.needs != null)
                            {
                                sb.Append(String.Format("\nNeeds :\n{0}\n", item.needs));
                            }
                            if (Convert.ToBoolean(Program.settings["Barters_and_Crafts"]) && item.bartersandcrafts != null)
                            {
                                sb.Append(String.Format("\n\nBarters & Crafts :\n{0}\n", item.bartersandcrafts));
                            }
                            iteminfo_ball.Rows.Clear();
                            iteminfo_text.Text = sb.ToString().Trim();
                            setTextColors(item);
                            if (item.ballistic != null)
                            {
                                foreach (Ballistic b in item.ballistic.Calibarlist)
                                {
                                    iteminfo_ball.Rows.Add(b.Data());
                                }
                                iteminfo_ball.Visible = true;
                                SetBallisticsColor(item);
                                ResizeGrid(iteminfo_ball);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        public void ShowInfoAPI(TarkovAPI.Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        if (item == null || item.name == null)
                        {
                            if (MainForm.timer.Enabled || MainForm.WaitingForTooltip)
                                iteminfo_text.Text = waitinfForTooltipText;
                            else
                                iteminfo_text.Text = Program.notfound;
                        }
                        else if (item.link == null)
                        {
                            iteminfo_text.Text = Program.noflea;
                        }
                        else
                        {
                            Debug.WriteLine("Market Link: " + item.link);

                            string mainCurrency = Program.rouble.ToString();
                            string BestSellTo_vendorName = "";
                            string BestBuyFrom_vendorName = "";
                            
                            StringBuilder sb = new StringBuilder();
                            
                            //Loot Tier
                            SetLootTierPerSlot(item);
                            if(item.lootTier != null)
                                sb.Append(String.Format("{0}", item.lootTier));

                            //Name
                            sb.Append(String.Format("\n{0}", item.name));

                            //Helmet/Armour Class
                            if (item.properties != null && item.properties._class != null)
                            {
                                item.className = "Class " + item.properties._class.Value;
                                if(item.properties._class.Value > 0)
                                sb.Append(String.Format("\n{0}\n", item.className));
                            }

                            //Key Info
                            if (item.types.Any(e => e.Equals("keys")) && item.wikiLink != null)
                            {
                                string lockLocation = FindKeyInfo(item);
                                if(lockLocation != null)
                                    sb.Append(String.Format("\n\nLocation: \n{0}", lockLocation));
                            }

                            //Find Flea Market profit
                            int flea_profit = 0;
                            if (item.lastLowPrice != null && item.fleaMarketFee != null)
                            {
                                flea_profit = item.lastLowPrice.Value - item.fleaMarketFee.Value;
                            }

                            //Find best trader to sell to
                            if (item.sellFor.Count > 0)
                            {
                                List<SellFor> list = new List<SellFor>(item.sellFor);
                                List<SellFor> sortedVendor = new List<SellFor>(list.OrderByDescending(p => p.priceRUB));
                                var lastSortedVendor = item.sellFor[0];

                                if (sortedVendor[0].vendor.name == "Flea Market" && sortedVendor.Count > 1 && flea_profit > 0)
                                {
                                    if (flea_profit > sortedVendor[1].priceRUB.Value)
                                    {
                                        lastSortedVendor = sortedVendor[0];
                                    }
                                    else
                                    {
                                        lastSortedVendor = sortedVendor[1];
                                    }
                                }

                                BestSellTo_vendorName = lastSortedVendor.vendor.name;

                                int vendorPrice = lastSortedVendor.priceRUB.Value;
                                if (lastSortedVendor.vendor.name == "Flea Market" && item.lastLowPrice != null)
                                    vendorPrice = flea_profit;

                                if (lastSortedVendor.vendor.minTraderLevel != null)
                                    BestSellTo_vendorName += " LL" + lastSortedVendor.vendor.minTraderLevel;

                                if (vendorPrice > 0)
                                    sb.Append(String.Format("\nBest sell to {0} --> {1}{2}", BestSellTo_vendorName, vendorPrice.ToString("N0"), mainCurrency));
                            }

                            //Find best trader to buy from
                            if (item.buyFor.Count > 0)
                            {
                                List<BuyFor> list = new List<BuyFor>(item.buyFor);
                                var sortedVendor = list.OrderBy(p => p.priceRUB).First();
                                BestBuyFrom_vendorName = sortedVendor.vendor.name;

                                int vendorPrice = sortedVendor.priceRUB.Value;
                                if (sortedVendor.vendor.name == "Flea Market" && item.lastLowPrice != null)
                                    vendorPrice = item.lastLowPrice.Value;

                                if (sortedVendor.vendor.minTraderLevel != null)
                                    BestBuyFrom_vendorName += " LL" + sortedVendor.vendor.minTraderLevel;

                                if(vendorPrice > 0)
                                    sb.Append(String.Format("\nBest buy from {0} --> {1}{2}", BestBuyFrom_vendorName, vendorPrice.ToString("N0"), mainCurrency));
                            }

                            if (Convert.ToBoolean(Program.settings["Show_Last_Price"]) && item.lastLowPrice != null)
                            {
                                sb.Append(String.Format("\n\nLast Price : {0}{1}  ({2})", ((int)item.lastLowPrice).ToString("N0"), mainCurrency, Program.LastUpdated((DateTime)item.updated)));
                            }
                            if (item.fleaMarketFee != null && !item.types.Exists(e => e.Equals("preset")))
                            {
                                if (flea_profit > 0)
                                    sb.Append(String.Format("\nProfit : {0}{1} (Fee : {2}{3})", flea_profit.ToString("N0"), mainCurrency, item.fleaMarketFee.Value.ToString("N0"), mainCurrency));
                            }
                            if (Convert.ToBoolean(Program.settings["Show_Day_Price"]) && item.avg24hPrice.Value > 0)
                            {
                                sb.Append(String.Format("\nAverage 24h : {0}{1}", item.avg24hPrice.Value.ToString("N0"), mainCurrency));
                            }
                            if (Convert.ToBoolean(Program.settings["Sell_to_Trader"]) && item.sellFor.Count > 0)
                            {
                                List<SellFor> list = new List<SellFor>(item.sellFor);
                                list.RemoveAll(p => p.vendor.name == "Flea Market");
                                if (list.Count > 0)
                                {
                                    var sortedNoFlea = list.OrderByDescending(p => p.priceRUB).First();
                                    string vendorName = sortedNoFlea.vendor.name;

                                    if (sortedNoFlea.vendor.minTraderLevel != null)
                                        vendorName += " LL" + sortedNoFlea.vendor.minTraderLevel;

                                    if(BestSellTo_vendorName != vendorName)
                                        sb.Append(String.Format("\n\nSell to {0} --> {1}{2}", vendorName, sortedNoFlea.priceRUB.Value.ToString("N0"), mainCurrency));
                                }
                            }
                            if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.buyFor.Count > 0)
                            {
                                List<BuyFor> list =  new List<BuyFor>(item.buyFor);
                                list.RemoveAll(p => p.vendor.name == "Flea Market");
                                if (list.Count > 0)
                                {
                                    var sortedNoFlea = list.OrderBy(p => p.priceRUB).First();
                                    string vendorName = sortedNoFlea.vendor.name;

                                    if (sortedNoFlea.vendor.minTraderLevel != null)
                                        vendorName += " LL" + sortedNoFlea.vendor.minTraderLevel;
                                    if(BestBuyFrom_vendorName != vendorName)
                                        sb.Append(String.Format("\nBuy from {0} --> {1}{2}", vendorName, sortedNoFlea.priceRUB.Value.ToString("N0"), mainCurrency));
                                }
                            }
                            if (Convert.ToBoolean(Program.settings["Needs"]) && item.usedInTasks.Count > 0)
                            {
                                string tasks = "";
                                var list = item.usedInTasks.OrderBy(p => p.minPlayerLevel);
                                foreach (var task in list)
                                {
                                    if (task.minPlayerLevel != null)
                                        tasks += "[" + task.minPlayerLevel + "] ";
                                    if (task.name != null)
                                        tasks += task.name;
                                    if (task.map != null)
                                        tasks += " [" + task.map.name + "]";
                                    tasks += "\n";
                                }

                                sb.Append(String.Format("\n\nUsed in Task:\n{0}", tasks));
                            }
                            //TODO
                            /*if (Convert.ToBoolean(Program.settings["Barters_and_Crafts"]) && item.bartersandcrafts != null)
                            {
                                sb.Append(String.Format("\n\nBarters & Crafts :\n{0}\n", item.bartersandcrafts));
                            }*/
                            if (item.types.Exists(e => e.Equals("preset")))
                            {
                                sb.Append(String.Format("\nThis is a Preset item \nCan't be sold or bought in Flea Market"));
                            }
                            else if (flea_profit == 0)
                                sb.Append(String.Format("\nItem Banned from Flea Market"));

                            iteminfo_ball.Rows.Clear();
                            iteminfo_text.Text = sb.ToString().Trim();
                            setTextColorsAPI(item);
                            if (item.ballistic != null)
                            {
                                foreach (Ballistic b in item.ballistic.Calibarlist)
                                {
                                    iteminfo_ball.Rows.Add(b.Data());
                                }
                                iteminfo_ball.Visible = true;
                                SetBallisticsColorAPI(item);
                                ResizeGrid(iteminfo_ball);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        private void SetLootTierPerSlot(TarkovAPI.Item item)
        {
            if (item.lastLowPrice != null || item.lastLowPrice > 0)
            {
                float valuePerSlot;
                if (item.types.Exists(e => e.Equals("gun")))
                    valuePerSlot = item.lastLowPrice.Value / (item.properties.defaultHeight.Value * item.properties.defaultWidth.Value);
                else
                    valuePerSlot = item.lastLowPrice.Value / (item.height.Value * item.width.Value);

                if (!item.types.Exists(e => e.Equals("ammo")))
                {
                    if (valuePerSlot < 8500)
                        item.lootTier = "Loot Tier F";
                    else if (valuePerSlot >= 8500 && valuePerSlot < 21000)
                        item.lootTier = "Loot Tier E";
                    else if (valuePerSlot >= 21000 && valuePerSlot < 26750)
                        item.lootTier = "Loot Tier D";
                    else if (valuePerSlot >= 26750 && valuePerSlot < 34250)
                        item.lootTier = "Loot Tier C";
                    else if (valuePerSlot >= 34250 && valuePerSlot < 45500)
                        item.lootTier = "Loot Tier B";
                    else if (valuePerSlot >= 45500 && valuePerSlot < 63000)
                        item.lootTier = "Loot Tier A";
                    else if (valuePerSlot >= 63000 && valuePerSlot < 100000)
                        item.lootTier = "Loot Tier S";
                    else if (valuePerSlot >= 100000 && valuePerSlot < 500000)
                        item.lootTier = "Loot Tier S+";
                    else if (valuePerSlot >= 500000 && valuePerSlot < 2000000)
                        item.lootTier = "Loot Tier S++";
                }
            }
        }

        private string FindKeyInfo(TarkovAPI.Item item)
        {
            try
            {
                using (TPVWebClient wc = new TPVWebClient())
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(wc.DownloadString(item.wikiLink));
                    HtmlAgilityPack.HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");
                    var subnode = node.SelectSingleNode("//p[3]");
                    return subnode.InnerText;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return null;
        }

        public void ShowCompare(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        DataGridViewRow temp = CheckItemExist(item);
                        if (item != null && item.market_address != null)
                        {
                            if (temp != null)
                            {
                                ItemCompareGrid.Rows.Remove(temp);
                            }
                            ItemCompareGrid.Rows.Add(item.Data());
                            if (ItemCompareGrid.SortedColumn != null)
                            {
                                ItemCompareGrid.Sort(ItemCompareGrid.SortedColumn,
                                    ItemCompareGrid.SortOrder.Equals(SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
                            }
                            ItemCompareGrid.Visible = true;
                            ResizeGrid(ItemCompareGrid);
                        }
                        if (temp == null)
                        {
                            if (--compare_size > 0)
                            {
                                itemcompare_text.Text = String.Format("{0} Left : {1}", Program.loading, compare_size);
                            }
                            else
                            {
                                itemcompare_text.Text = String.Format("{0}", Program.presscomparekey);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }

        //TODO
        /*public void ShowCompareAPI(TarkovAPI.Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        DataGridViewRow temp = CheckItemExistAPI(item);
                        if (item != null && item.Link != null)
                        {
                            if (temp != null)
                            {
                                ItemCompareGrid.Rows.Remove(temp);
                            }
                            ItemCompareGrid.Rows.Add(item.Data());
                            if (ItemCompareGrid.SortedColumn != null)
                            {
                                ItemCompareGrid.Sort(ItemCompareGrid.SortedColumn,
                                    ItemCompareGrid.SortOrder.Equals(SortOrder.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending);
                            }
                            ItemCompareGrid.Visible = true;
                            ResizeGrid(ItemCompareGrid);
                        }
                        if (temp == null)
                        {
                            if (--compare_size > 0)
                            {
                                itemcompare_text.Text = String.Format("{0} Left : {1}", Program.loading, compare_size);
                            }
                            else
                            {
                                itemcompare_text.Text = String.Format("{0}", Program.presscomparekey);
                            }
                        }
                    }
                }
            };
            Invoke(show);
        }*/

        public DataGridViewRow CheckItemExist(Item item)
        {
            DataGridViewRow value = null;
            foreach (DataGridViewRow r in ItemCompareGrid.Rows)
            {
                if ((item.isname2 ? item.name_display2 : item.name_display).Equals(r.Cells[0].Value))
                {
                    value = r;
                    break;
                }
            }
            return value;
        }

        public DataGridViewRow CheckItemExistAPI(TarkovAPI.Item item)
        {
            DataGridViewRow value = null;
            foreach (DataGridViewRow r in ItemCompareGrid.Rows)
            {
                if ((item.name).Equals(r.Cells[0].Value))
                {
                    value = r;
                    break;
                }
            }
            return value;
        }

        public void setTextColors(Item item)
        {
            setPriceColor();
            setInraidColor();
            setCraftColor(item);
            setOthersColor(item);
        }

        public void setTextColorsAPI(TarkovAPI.Item item)
        {
            setPriceColor();
            setInraidColor();
            
            setOthersColorAPI(item);
            setCraftColorAPI(item);
            setLootTierColorAPI(item);
            setClassTierColorAPI(item);
        }

        public void setPriceColor()
        {
            MatchCollection mc = Program.money_filter.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Gold;
            }
        }

        public void setInraidColor()
        {
            MatchCollection mc = Program.inraid_filter.Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Red;
            }
        }

        public void setCraftColor(Item item)
        { 

            MatchCollection mc;

            if (iteminfo_text.Text.Contains(item.name_display2) && item.name_display2 != null)
            {
                mc = new Regex(Regex.Escape(item.name_display2)).Matches(iteminfo_text.Text);
            }
            else
                mc = new Regex(Regex.Escape(item.name_display)).Matches(iteminfo_text.Text);

            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Green;
            }
        }

        public void setCraftColorAPI(TarkovAPI.Item item)
        {
            //MatchCollection mc = new Regex(Regex.Escape(item.name)).Matches(iteminfo_text.Text);
            MatchCollection mc = new Regex(Regex.Escape(item.name)).Matches(iteminfo_text.Text);

            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.ForestGreen;
            }
        }

        public void setLootTierColorAPI(TarkovAPI.Item item)
        {
            if (item.lootTier != null)
            {
                if (item.lootTier.Contains("F"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.Gray;
                    }
                }
                else if (item.lootTier.Contains("E"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.DarkGoldenrod;
                    }
                }
                else if (item.lootTier.Contains("D"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.DarkGreen;
                    }
                }
                else if (item.lootTier.Contains("C"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.LimeGreen;
                    }
                }
                else if (item.lootTier.Contains("B"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.RoyalBlue;
                    }
                }
                else if (item.lootTier.Contains("A"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.DarkViolet;
                    }
                }
                else if (item.lootTier.Contains("S"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.lootTier)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.Gold;
                    }
                }
            }
        }

        public void setClassTierColorAPI(TarkovAPI.Item item)
        {
            if (item.className != null)
            {
                if (item.className.Contains("1"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.DarkGoldenrod;
                    }
                }
                else if (item.className.Contains("2"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.LimeGreen;
                    }
                }
                else if (item.className.Contains("3"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.RoyalBlue;
                    }
                }
                else if (item.className.Contains("4"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.DarkViolet;
                    }
                }
                else if (item.className.Contains("5"))
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.Gold;
                    }
                }
                else
                {
                    MatchCollection mc = new Regex(Regex.Escape(item.className)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.Gold;
                    }
                }
            }
        }

        public void setOthersColor(Item item)
        {
            MatchCollection mc = new Regex(Regex.Escape(item.sell_to_trader)).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.SkyBlue;
            }

            if (item.buy_from_trader != null)
            {
                mc = new Regex(Regex.Escape(item.buy_from_trader)).Matches(iteminfo_text.Text);
                foreach (Match m in mc)
                {
                    iteminfo_text.Select(m.Index, m.Length);
                    iteminfo_text.SelectionColor = Color.SkyBlue;
                }
            }
            
            mc = new Regex("Flea Market").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.SkyBlue;
            }

            mc = new Regex("Banned from Flea Market").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Red;
            }
        }

        public void setOthersColorAPI(TarkovAPI.Item item)
        {
            MatchCollection mc;

            foreach (var itemT in item.sellFor)
            {
                mc = new Regex(Regex.Escape(itemT.vendor.name)).Matches(iteminfo_text.Text);
                foreach (Match m in mc)
                {
                    iteminfo_text.Select(m.Index, m.Length);
                    iteminfo_text.SelectionColor = Color.SkyBlue;
                }
            }

            foreach (var itemT in item.buyFor)
            {
                if (itemT.vendor != null)
                {
                    mc = new Regex(Regex.Escape(itemT.vendor.name)).Matches(iteminfo_text.Text);
                    foreach (Match m in mc)
                    {
                        iteminfo_text.Select(m.Index, m.Length);
                        iteminfo_text.SelectionColor = Color.SkyBlue;
                    }
                }
            }
            

            mc = new Regex("Flea Market").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.SkyBlue;
            }

            mc = new Regex("Item Banned from Flea Market").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Red;
            }

            mc = new Regex("This is a Preset item \nCan't be sold or bought in Flea Market").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Orange;
            }

            mc = new Regex("Used in Task:").Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.DarkOrange;
            }
            
            mc = new Regex(Regex.Escape("[")).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.DarkOrange;
            }

            mc = new Regex(Regex.Escape("]")).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.DarkOrange;
            }

            mc = new Regex(Regex.Escape("Location:")).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.SandyBrown;
            }

            string[] mapList = { "Customs", "Interchange", "Factory", "Woods", "Reserve", "Shoreline", "The Lab", "Lighthouse", "Streets of Tarkov" };
            foreach (var map in mapList)
            {
                mc = new Regex(map).Matches(iteminfo_text.Text);
                foreach (Match m in mc)
                {
                    iteminfo_text.Select(m.Index, m.Length);
                    iteminfo_text.SelectionColor = Color.SteelBlue;
                }
            }
        }

        public void ShowLoadingInfo(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    //waitinfForTooltipText = Program.waitingForTooltip;
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;
                    iteminfo_text.Text = Program.loading;
                    iteminfo_panel.Location = new Point(point.X + 20, point.Y + 20);
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowWaitinfForTooltipInfo(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;

                    if (DotsCounter < 3)
                    {
                        waitinfForTooltipText += ".";
                        DotsCounter++;
                    }
                    else
                    {
                        waitinfForTooltipText = Program.waitingForTooltip + ".";
                        DotsCounter = 1;
                    }
                    iteminfo_panel.Location = new Point(point.X + 20, point.Y + 20);
                    iteminfo_text.Text = waitinfForTooltipText;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowWaitBallistics(Point point)
        {
            Action show = delegate ()
            {
                lock (_lock)
                {
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;
                    iteminfo_text.Text = Program.notfinishloading;
                    iteminfo_panel.Location = point;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowWaitAPI(Point point)
        {
            Action show = delegate ()
            {
                lock (_lock)
                {
                    iteminfo_ball.Rows.Clear();
                    iteminfo_ball.Visible = false;
                    iteminfo_text.Text = Program.notfinishloadingAPI;
                    iteminfo_panel.Location = point;
                    iteminfo_panel.Visible = true;
                }
            };
            Invoke(show);
        }

        public void ShowLoadingCompare(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    lock (_lock)
                    {
                        if (!itemcompare_panel.Visible)
                        {
                            compare_size = 0;
                            ItemCompareGrid.Rows.Clear();
                            ResizeGrid(ItemCompareGrid);
                            itemcompare_panel.Location = point;
                            itemcompare_panel.Visible = true;
                            itemcompare_text.Text = String.Format("{0}", Program.presscomparekey);
                        }
                        itemcompare_text.Text = String.Format("{0} Left : {1}", Program.loading, ++compare_size);
                    }
                }
            };
            Invoke(show);
        }

        public void HideInfo()
        {
            Action show = delegate ()
            {
                iteminfo_ball.Visible = false;
                iteminfo_panel.Visible = false;
            };
            Invoke(show);
        }

        public void HideCompare()
        {
            Action show = delegate ()
            {
                ItemCompareGrid.Visible = false;
                itemcompare_panel.Visible = false;
            };
            Invoke(show);
        }

        public void ChangeTransparent(int value)
        {
            Action show = delegate ()
            {
                this.Opacity = value * 0.01;
            };
            Invoke(show);
        }

        private void FixLocation(Control p)
        {
            int totalwidth = p.Location.X + p.Width;
            int totalheight = p.Location.Y + p.Height;
            int x = p.Location.X;
            int y = p.Location.Y;
            if (totalwidth > this.Width)
            {
                x -= totalwidth - this.Width;
            }
            if (totalheight > this.Height)
            {
                y -= totalheight - this.Height;
            }
            if (x != p.Location.X || y != p.Location.Y)
            {
                p.Location = new Point(x, y);
            }
            p.Refresh();
        }

        private void itemwindow_panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void itemwindow_panel_SizeChanged(object sender, EventArgs e)
        {
            iteminfo_ball.Location = new Point(iteminfo_text.Location.X, iteminfo_text.Location.Y + iteminfo_text.Height + 15);
            FixLocation(sender as Control);
        }

        private void itemwindow_panel_LocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void itemwindow_text_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            (sender as Control).ClientSize = new Size(e.NewRectangle.Width + 1, e.NewRectangle.Height + 1);
        }

        private void itemcompare_text_MouseMove(object sender, MouseEventArgs e)
        {
            if (ismoving)
            {
                itemcompare_panel.Location = new Point(Cursor.Position.X - x, Cursor.Position.Y - y);
            }
        }

        private void itemcompare_text_MouseDown(object sender, MouseEventArgs e)
        {
            x = Cursor.Position.X - itemcompare_panel.Location.X;
            y = Cursor.Position.Y - itemcompare_panel.Location.Y;
            ismoving = true;
        }

        private void itemcompare_text_MouseUp(object sender, MouseEventArgs e)
        {
            ismoving = false;
        }
    }
}
