using System.Windows.Forms;

namespace TarkovPriceViewer.UI
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
            this.MouseDown += KeyPressCheck_MouseDown;
            this.KeyDown += KeyPressCheck_KeyDown;
            this.Text = "Press a key or mouse button (you can hold Shift/Ctrl/Alt)";
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
                    int primary = (int)e.KeyCode;
                    string bind = BuildKeyboardKeyBindString(e);
                    switch (button)
                    {
                        case 1:
                            Program.AppSettings.ShowOverlayKeyBind = bind;
                            break;
                        case 2:
                            Program.AppSettings.HideOverlayKeyBind = bind;
                            break;
                        case 3:
                            Program.AppSettings.CompareOverlayKeyBind = bind;
                            break;
                        case 4:
                            Program.AppSettings.IncreaseTrackerCountKeyBind = bind;
                            break;
                        case 5:
                            Program.AppSettings.DecreaseTrackerCountKeyBind = bind;
                            break;
                        case 6:
                            Program.AppSettings.ToggleFavoriteItemKeyBind = bind;
                            break;
                        case 4:
                            Program.AppSettings.IncreaseTrackerCountKey = (int)e.KeyCode;
                            break;
                        case 5:
                            Program.AppSettings.DecreaseTrackerCountKey = (int)e.KeyCode;
                            break;
                    }
                    // Persist key changes immediately so they survive future loads
                    Program.SaveSettings();
                    if (Owner != null)
                    {
                        ((MainForm)Owner).ChangePressKeyData(null);
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
                string bind = BuildKeyBindString(mouseCode, true);
                switch (button)
                {
                    case 1:
                        Program.AppSettings.ShowOverlayKeyBind = bind;
                        break;
                    case 2:
                        Program.AppSettings.HideOverlayKeyBind = bind;
                        break;
                    case 3:
                        Program.AppSettings.CompareOverlayKeyBind = bind;
                        break;
                    case 4:
                        Program.AppSettings.IncreaseTrackerCountKeyBind = bind;
                        break;
                    case 5:
                        Program.AppSettings.DecreaseTrackerCountKeyBind = bind;
                        break;
                    case 6:
                        Program.AppSettings.ToggleFavoriteItemKeyBind = bind;
                        break;
                    case 4:
                        Program.AppSettings.IncreaseTrackerCountKey = mouseCode;
                        break;
                    case 5:
                        Program.AppSettings.DecreaseTrackerCountKey = mouseCode;
                        break;
                }

                // Persist mouse-based key changes as well
                Program.SaveSettings();
                if (Owner != null)
                {
                    ((MainForm)Owner).ChangePressKeyData(null);
                }

                this.Close();
            }
        }

        private void KeyPressCheck_KeyDown(object sender, KeyEventArgs e)
        {
            // If it's a pure modifier, just refresh the modifier preview
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
            {
                UpdateWindowTitle();
                return;
            }

            // For a normal key, show modifiers + key without a trailing '+'
            var parts = new System.Collections.Generic.List<string>();
            if ((ModifierKeys & Keys.Shift) != 0)
            {
                parts.Add("Shift");
            }
            if ((ModifierKeys & Keys.Control) != 0)
            {
                parts.Add("Ctrl");
            }
            if ((ModifierKeys & Keys.Alt) != 0)
            {
                parts.Add("Alt");
            }

            parts.Add(e.KeyCode.ToString());
            label1.Text = string.Join(" + ", parts);
        }

        private void KeyPressCheck_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateWindowTitle();
        }

        private void UpdateWindowTitle()
        {
            var parts = new System.Collections.Generic.List<string>();

            if ((ModifierKeys & Keys.Shift) != 0)
            {
                parts.Add("Shift");
            }
            if ((ModifierKeys & Keys.Control) != 0)
            {
                parts.Add("Ctrl");
            }
            if ((ModifierKeys & Keys.Alt) != 0)
            {
                parts.Add("Alt");
            }

            if ((MouseButtons & MouseButtons.Left) != 0)
            {
                parts.Add("Mouse Left");
            }
            if ((MouseButtons & MouseButtons.Right) != 0)
            {
                parts.Add("Mouse Right");
            }
            if ((MouseButtons & MouseButtons.Middle) != 0)
            {
                parts.Add("Mouse Middle");
            }
            if ((MouseButtons & MouseButtons.XButton1) != 0)
            {
                parts.Add("Mouse X1");
            }
            if ((MouseButtons & MouseButtons.XButton2) != 0)
            {
                parts.Add("Mouse X2");
            }

            bool hasMouse = (MouseButtons & (MouseButtons.Left | MouseButtons.Right | MouseButtons.Middle | MouseButtons.XButton1 | MouseButtons.XButton2)) != 0;

            if (parts.Count == 0)
            {
                label1.Text = "Press Key";
            }
            else
            {
                var text = string.Join(" + ", parts);
                // If there are only modifiers (no mouse), show a trailing '+' to indicate that another key is expected
                if (!hasMouse)
                {
                    text += " +";
                }
                label1.Text = text;
            }
        }

        private string BuildKeyboardKeyBindString(KeyEventArgs e)
        {
            var codes = new System.Collections.Generic.List<int>();

            if (e.Shift)
            {
                codes.Add((int)Keys.ShiftKey);
            }
            if (e.Control)
            {
                codes.Add((int)Keys.ControlKey);
            }
            if (e.Alt)
            {
                codes.Add((int)Keys.Menu);
            }

            if (e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.Menu)
            {
                codes.Add((int)e.KeyCode);
            }

            if (codes.Count == 0)
            {
                return string.Empty;
            }

            codes.Sort();
            return string.Join("+", codes);
        }

        private string BuildKeyBindString(int primaryCode, bool isMouse)
        {
            var codes = new System.Collections.Generic.List<int>();

            if ((ModifierKeys & Keys.Shift) != 0)
            {
                codes.Add((int)Keys.ShiftKey);
            }
            if ((ModifierKeys & Keys.Control) != 0)
            {
                codes.Add((int)Keys.ControlKey);
            }
            if ((ModifierKeys & Keys.Alt) != 0)
            {
                codes.Add((int)Keys.Menu);
            }

            if (isMouse)
            {
                if ((MouseButtons & MouseButtons.Left) != 0 || primaryCode == MOUSE_LEFT)
                {
                    codes.Add(MOUSE_LEFT);
                }
                if ((MouseButtons & MouseButtons.Right) != 0 || primaryCode == MOUSE_RIGHT)
                {
                    codes.Add(MOUSE_RIGHT);
                }
                if ((MouseButtons & MouseButtons.Middle) != 0 || primaryCode == MOUSE_MIDDLE)
                {
                    codes.Add(MOUSE_MIDDLE);
                }
                if ((MouseButtons & MouseButtons.XButton1) != 0 || primaryCode == MOUSE_X1)
                {
                    codes.Add(MOUSE_X1);
                }
                if ((MouseButtons & MouseButtons.XButton2) != 0 || primaryCode == MOUSE_X2)
                {
                    codes.Add(MOUSE_X2);
                }
            }
            else
            {
                if (primaryCode != (int)Keys.ShiftKey && primaryCode != (int)Keys.ControlKey && primaryCode != (int)Keys.Menu)
                {
                    codes.Add(primaryCode);
                }
            }

            if (codes.Count == 0)
            {
                return string.Empty;
            }

            codes.Sort();
            return string.Join("+", codes);
        }
    }
}
