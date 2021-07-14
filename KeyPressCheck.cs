using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        private int button;

        public KeyPressCheck(MainForm main, int button)
        {
            this.main = main;
            this.button = button;
            InitializeComponent();
        }

        private void KeyPressCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu && e.KeyCode != Keys.ControlKey)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    /*StringBuilder sb = new StringBuilder();
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
                    Program.ShowOverlay_Key = sb.ToString();*/
                    switch (button)
                    {
                        case 1:
                            Program.ShowOverlay_Key = (int)e.KeyCode;
                            break;
                        case 2:
                            Program.HideOverlay_Key = (int)e.KeyCode;
                            break;
                    }
                }
                main.ChangePressKeyData();
                this.Close();
            }
        }
    }
}
