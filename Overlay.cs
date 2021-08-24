using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

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
        private static readonly DataTable comparetable = new DataTable();
        private static int compare_size = 0;

        public Overlay()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Opacity = Int32.Parse(Program.settings["Overlay_Transparent"]) * 0.01;
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            settingFormPos();
            initializeCompareData();
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
            comparetable.Columns.Add("Name", typeof(string));
            comparetable.Columns.Add("Recoil", typeof(string));
            comparetable.Columns.Add("Accuracy", typeof(string));
            comparetable.Columns.Add("Ergo", typeof(string));
            comparetable.Columns.Add("Flea", typeof(string));
            comparetable.Columns.Add("NPC", typeof(string));
            comparetable.Columns.Add("LL", typeof(string));
            ItemCompareGrid.Visible = false;
            ItemCompareGrid.DataSource = comparetable;
            ItemCompareGrid.ClearSelection();
            ResizeGrid();
        }

        public void ResizeGrid()
        {
            ItemCompareGrid.ClientSize = new Size(ItemCompareGrid.Columns.GetColumnsWidth(DataGridViewElementStates.None) + 10,
                ItemCompareGrid.Rows.GetRowsHeight(DataGridViewElementStates.None) + 20);
            ItemCompareGrid.Refresh();
        }

        public void ShowInfo(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    if (item == null || item.market_address == null)
                    {
                        iteminfo_text.Text = Program.notfound;
                    }
                    else if (item.price_last == null)
                    {
                        iteminfo_text.Text = Program.noflea;
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("Name : {0}\n\n", item.isname2 ? item.name_display2 : item.name_display));
                        if (Convert.ToBoolean(Program.settings["Show_Last_Price"]))
                        {
                            sb.Append(String.Format("Last Price : {0} ({1})\n", item.price_last, item.last_updated));
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
                            sb.Append(String.Format("\nSell to Trader : {0}", item.sell_to_trader));
                            sb.Append(String.Format("\nSell to Trader Price : {0}\n", item.sell_to_trader_price));
                        }
                        if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.buy_from_trader != null)
                        {
                            sb.Append(String.Format("\nBuy From Trader : {0}", item.buy_from_trader));
                            sb.Append(String.Format("\nBuy From Trader Price : {0}\n", item.buy_from_trader_price.Replace(" ", "").Replace(@"~", @" ≈")));
                        }
                        if (Convert.ToBoolean(Program.settings["Needs"]) && item.needs != null)
                        {
                            sb.Append(String.Format("\nNeeds :\n{0}\n", item.needs));
                        }
                        if (Convert.ToBoolean(Program.settings["Barters_and_Crafts"]) && item.bartersandcrafts != null)
                        {
                            sb.Append(String.Format("\nBarters & Crafts :\n{0}\n", item.bartersandcrafts));
                        }
                        iteminfo_text.Text = sb.ToString().Trim();
                        setTextColors(item);
                    }
                }
            };
            Invoke(show);
        }

        public void ShowCompare(Item item, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    if (--compare_size > 0)
                    {
                        itemcompare_text.Text = String.Format("{0} Left : {1}", Program.loading, compare_size);
                    } else
                    {
                        itemcompare_text.Text = String.Format("{0}", Program.presscomparekey);
                    }
                    comparetable.Rows.Add(item.Data());
                    ItemCompareGrid.Visible = true;
                    ResizeGrid();
                }
            };
            Invoke(show);
        }

        public void setTextColors(Item item)
        {
            setPriceColor();
            setInraidColor();
            setCraftColor(item);
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
            MatchCollection mc = new Regex(item.name_display).Matches(iteminfo_text.Text);
            foreach (Match m in mc)
            {
                iteminfo_text.Select(m.Index, m.Length);
                iteminfo_text.SelectionColor = Color.Green;
            }
        }

        public void ShowLoadingInfo(Point point, CancellationToken cts_one)
        {
            Action show = delegate ()
            {
                if (!cts_one.IsCancellationRequested)
                {
                    iteminfo_text.Text = Program.loading;
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
                    if (!itemcompare_panel.Visible)
                    {
                        compare_size = 0;
                        comparetable.Rows.Clear();
                        ResizeGrid();
                        itemcompare_panel.Location = point;
                        itemcompare_panel.Visible = true;
                        itemcompare_text.Text = String.Format("{0}", Program.presscomparekey);
                    }
                    itemcompare_text.Text = String.Format("{0} Left : {1}", Program.loading, ++compare_size);
                }
            };
            Invoke(show);
        }

        public void HideInfo()
        {
            Action show = delegate ()
            {
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
    }
}
