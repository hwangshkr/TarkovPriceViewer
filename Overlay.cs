using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TarkovPriceViewer
{
    public partial class Overlay : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_LAYERED = 0x80000;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const String rouble = "₽";
        private const String dollar = "$";

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

        public void ShowInfo(MainForm.Item item, Point point)
        {
            Action show = delegate ()
            {
                iteminfo_panel.Visible = false;
                itemusage.Visible = false;
                iteminfo_panel.Location = point;
                itemname.Text = "Name : " + new string(item.name);
                itemprice.Text = "Flea Price : " + rouble + item.price;
                itemtrader.Text = "Trader : " + item.trader;
                traderprice.Text = "Trader Price : " + item.currency + item.trader_price;
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

        public void VisibleUsage(String str)
        {
            Action show = delegate ()
            {
                itemusage.Text = str;
                itemusage.Visible = true;
            };
            Invoke(show);
        }

        private void iteminfo_panel_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, (sender as Panel).ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void iteminfo_panel_SizeChanged(object sender, EventArgs e)
        {
            (sender as Panel).Refresh();
        }
    }
}
