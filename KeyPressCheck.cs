using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TarkovPriceViewer
{
    public partial class KeyPressCheck : Form
    {
        private MainForm main;

        public KeyPressCheck(MainForm main)
        {
            this.main = main;
            InitializeComponent();
        }

        private void KeyPressCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Shift && e.KeyCode != Keys.Alt && e.KeyCode != Keys.Control)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    StringBuilder sb = new StringBuilder();
                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("+");
                        }
                        sb.Append(Keys.Shift.ToString());
                    }
                    if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("+");
                        }
                        sb.Append(Keys.Alt.ToString());
                    }
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        if (sb.Length > 0)
                        {
                            sb.Append("+");
                        }
                        sb.Append(Keys.Control.ToString());
                    }
                    if (sb.Length > 0)
                    {
                        sb.Append("+");
                    }
                    sb.Append(e.KeyCode.ToString());
                    Program.ShowOverlay_Key = sb.ToString();
                }
                main.ChangePressKeyData();
                this.Close();
            }
        }
    }
}
