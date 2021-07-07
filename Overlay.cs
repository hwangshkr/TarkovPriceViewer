﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;
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
        //private static readonly String rouble = "₽";
        //private static readonly String dollar = "$";

        public Overlay()
        {
            InitializeComponent();
            this.TopMost = true;
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
            settingFormPos();
            iteminfo_panel.Visible = false;
        }

        public void settingFormPos()
        {
            this.Location = new Point(0, 0);
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void ShowInfo(Item item, Point point)
        {
            Action show = delegate ()
            {
                iteminfo_panel.Visible = false;
                iteminfo_panel.Location = point;
                itemname.Text = "Name : " + new string(item.name);
                itemprice.Text = "Flea Price : " + item.price + " (" + item.last_updated + ")";
                itemtrader.Text = "Trader : " + item.trader;
                traderprice.Text = "Trader Price : " + item.trader_price;
                itemusage.Text = item.Needs;
                if (item.Needs == null)
                {
                    itemusage.Visible = false;
                }
                else
                {
                    itemusage.Visible = true;
                }
                iteminfo_panel.Visible = true;
            };
            Invoke(show);
        }

        public bool isItemInfoVisible()
        {
            return iteminfo_panel.Visible;
        }

        public void setItemInfoVisible(bool isvisible)
        {
            iteminfo_panel.Visible = isvisible;
        }

        private void FixLocation(Panel p)
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
            ControlPaint.DrawBorder(e.Graphics, (sender as Panel).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void iteminfo_panel_SizeChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Panel);
        }

        private void iteminfo_panel_LocationChanged(object sender, EventArgs e)
        {
            FixLocation(sender as Panel);
        }
    }
}
