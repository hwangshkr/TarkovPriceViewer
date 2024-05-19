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

        private void KeyPressCheck_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Owner != null)
            {
                ((MainForm)Owner).ChangePressKeyData(null);
            }
        }

        private void KeyPressCheck_KeyUp(object sender, KeyEventArgs e)
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
