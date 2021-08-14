using System;
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

        public Overlay()
        {
            InitializeComponent();
            this.TopMost = true;
            this.Opacity = Int32.Parse(Program.settings["Overlay_Transparent"]) * 0.01;
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            settingFormPos();
            iteminfo_panel.Visible = false;
            onetext.Visible = false;
        }

        public void settingFormPos()
        {
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void ShowInfo(Item item, CancellationToken cts)
        {
            Action show = delegate ()
            {
                if (!cts.IsCancellationRequested)
                {
                    if (item == null || item.name_address == null)
                    {
                        onetext.Text = Program.notfound;
                    }
                    else if (item.price_last == null)
                    {
                        onetext.Text = Program.noflea;
                    }
                    else
                    {
                        iteminfo_panel.Location = onetext.Location;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(String.Format("Name : {0}", item.isname2 ? item.name_display2 : item.name_display));
                        if (Convert.ToBoolean(Program.settings["Show_Last_Price"]))
                        {
                            sb.Append(String.Format("\nLast Price : {0} ({1})", item.price_last, item.last_updated));
                        }
                        if (Convert.ToBoolean(Program.settings["Show_Day_Price"]) && item.price_day != null)
                        {
                            sb.Append(String.Format("\nDay Price : {0}", item.price_day));
                        }
                        if (Convert.ToBoolean(Program.settings["Show_Week_Price"]) && item.price_week != null)
                        {
                            sb.Append(String.Format("\nWeek Price : {0}", item.price_week));
                        }
                        if (Convert.ToBoolean(Program.settings["Sell_to_Trader"]) && item.sell_to_trader != null)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("\n");
                            }
                            sb.Append(String.Format("\nSell to Trader : {0}", item.sell_to_trader));
                            sb.Append(String.Format("\nSell to Trader Price : {0}", item.sell_to_trader_price));
                        }
                        if (Convert.ToBoolean(Program.settings["Buy_From_Trader"]) && item.buy_from_trader != null)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("\n");
                            }
                            sb.Append(String.Format("\nBuy From Trader : {0}", item.buy_from_trader));
                            sb.Append(String.Format("\nBuy From Trader Price : {0}", item.buy_from_trader_price));
                        }
                        if (item.Needs != null)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Append("\n");
                            }
                            sb.Append(String.Format("\nNeeds :\n{0}", item.Needs));
                            setInraidColor();
                        }
                        iteminfo_text.Text = sb.ToString();
                        onetext.Visible = false;
                        iteminfo_panel.Visible = true;
                    }
                }
            };
            Invoke(show);
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

        public void ShowLoadingInfo(Point point, CancellationToken cts)
        {
            Action show = delegate ()
            {
                if (!cts.IsCancellationRequested)
                {
                    onetext.Location = point;
                    onetext.Text = Program.loading;
                    iteminfo_panel.Visible = false;
                    onetext.Visible = true;
                }
            };
            Invoke(show);
        }

        public void HideInfo()
        {
            Action show = delegate ()
            {
                iteminfo_panel.Visible = false;
                onetext.Visible = false;
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

        private void iteminfo_panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void iteminfo_panel_SizeChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void iteminfo_panel_LocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void onetext_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Control).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void onetext_SizeChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void onetext_LocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Control);
        }

        private void Overlay_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void iteminfo_text_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            const int MARGIN = 5;
            RichTextBox richTextBox = sender as RichTextBox;
            richTextBox.ClientSize = new Size(e.NewRectangle.Width + MARGIN, e.NewRectangle.Height + MARGIN);
        }
    }
}
