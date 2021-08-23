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
        private int button;

        public KeyPressCheck(int button)
        {
            this.button = button;
            InitializeComponent();
        }

        private void KeyPressCheck_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu && e.KeyCode != Keys.ControlKey)
            {
                if (e.KeyCode != Keys.Escape)
                {
                    switch (button)
                    {
                        case 1:
                            Program.settings["ShowOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 2:
                            Program.settings["HideOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                        case 3:
                            Program.settings["CompareOverlay_Key"] = ((int)e.KeyCode).ToString();
                            break;
                    }
                    if (Owner != null)
                    {
                        ((MainForm)Owner).ChangePressKeyData(e.KeyCode);
                    }
                }
                this.Close();
            }
        }
    }
}
