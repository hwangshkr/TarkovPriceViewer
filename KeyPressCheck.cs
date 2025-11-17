using System.Windows.Forms;

namespace TarkovPriceViewer
{
    public partial class KeyPressCheck : Form
    {
        private const int MOUSE_LEFT = 1001;
        private const int MOUSE_RIGHT = 1002;
        private const int MOUSE_MIDDLE = 1003;
        private const int MOUSE_X1 = 1004;
        private const int MOUSE_X2 = 1005;
        private int button;

        public KeyPressCheck(int button)
        {
            this.button = button;
            InitializeComponent();
            this.MouseUp += KeyPressCheck_MouseUp;
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

        private void KeyPressCheck_MouseUp(object sender, MouseEventArgs e)
        {
            int mouseCode = 0;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    mouseCode = MOUSE_LEFT;
                    break;
                case MouseButtons.Right:
                    mouseCode = MOUSE_RIGHT;
                    break;
                case MouseButtons.Middle:
                    mouseCode = MOUSE_MIDDLE;
                    break;
                case MouseButtons.XButton1:
                    mouseCode = MOUSE_X1;
                    break;
                case MouseButtons.XButton2:
                    mouseCode = MOUSE_X2;
                    break;
            }

            if (mouseCode != 0)
            {
                switch (button)
                {
                    case 1:
                        Program.settings["ShowOverlay_Key"] = mouseCode.ToString();
                        break;
                    case 2:
                        Program.settings["HideOverlay_Key"] = mouseCode.ToString();
                        break;
                    case 3:
                        Program.settings["CompareOverlay_Key"] = mouseCode.ToString();
                        break;
                }

                if (Owner != null)
                {
                    ((MainForm)Owner).ChangePressKeyData(null);
                }

                this.Close();
            }
        }
    }
}

