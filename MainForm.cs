using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace TarkovPriceViewer
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        private enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }
        public struct Item
        {
            public char[] name;
            public int price;
            public String trader;
            public int trader_price;
            public String currency;
        }

        private static readonly int WH_KEYBOARD_LL = 13;
        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly LowLevelKeyboardProc _proc = hookProc;
        private static readonly Overlay overlay = new Overlay();
        private static readonly String appname = "EscapeFromTarkov";
        private static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        private static readonly List<Item> itemlist = new List<Item>();
        private static readonly WebClient wc = new WebClient();
        private static IntPtr hhook = IntPtr.Zero;
        private static int nFlags = 0x0;
        private static Bitmap fullimage = null;
        private static Thread backthread = null;
        private static System.Drawing.Point point = new System.Drawing.Point(0, 0);

        public MainForm()
        {
            InitializeComponent();
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
        }

        private void MainForm_load(object sender, EventArgs e)
        {
            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;
            if (v.Major == 10)
            {
                nFlags = 0x2;
            }
            TrayIcon.Visible = true;
            HideFormWhenStartup();
            wc.Encoding = Encoding.UTF8;
            getItemList();
            overlay.Show();
            SetHook();
        }

        private void HideFormWhenStartup()
        {
            this.Opacity = 0;
            this.Show();
            BeginInvoke(new MethodInvoker(delegate
            {
                this.Hide();
                this.Opacity = 1;
            }));
        }

        private void MainForm_closed(object sender, FormClosedEventArgs e)
        {
            TrayIcon.Dispose();
            UnHook();
            CloseItemInfo();
        }

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public static IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                switch (vkCode)
                {
                    case 120:
                        var cursor = Control.MousePosition;
                        point = new System.Drawing.Point(cursor.X, cursor.Y);
                        CloseItemInfo();
                        backthread = new Thread(BackWork);
                        backthread.IsBackground = true;
                        backthread.Start();
                        break;
                    case 9:
                    case 27:
                    case 121:
                        CloseItemInfo();
                        break;
                }
            }
            return CallNextHookEx(hhook, code, (int)wParam, lParam);
        }

        public static void CloseItemInfo()
        {
            if (backthread != null)
            {
                backthread.Abort();
            }
            overlay.setItemInfoVisible(false);
        }

        private static void BackWork()
        {
            if (CheckisTarkov())
            {
                CaptureScreen();
                if (fullimage != null)
                {
                    FindItemName();
                }
                else
                {
                    Debug.WriteLine("image null");
                }
            }
        }

        private static bool CheckisTarkov()
        {
            IntPtr hWnd = GetActiveWindow();
            if (hWnd != IntPtr.Zero)
            {
                StringBuilder sbWinText = new StringBuilder(260);
                GetWindowText(hWnd, sbWinText, 260);
                if (sbWinText.ToString() == appname)
                {
                    return true;
                }
            }
            Debug.WriteLine("error - no app");
            return true;
        }

        private static void CaptureScreen()
        {
            IntPtr findwindow = FindWindow(null, appname);
            if (findwindow != IntPtr.Zero)
            {
                Graphics Graphicsdata = Graphics.FromHwnd(findwindow);
                Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(findwindow, hdc, nFlags);
                    g.ReleaseHdc(hdc);
                }
                fullimage = bmp;
            }
            else
            {
                Debug.WriteLine("error - no window");
                fullimage = null;
#if DEBUG
                try
                {
                    fullimage = new Bitmap(@"img\test.png");
                } catch (Exception e)
                {
                    Debug.WriteLine("no test img" + e.Message);
                }
#endif
            }
        }

        private static String getTesseract(Mat textmat)
        {
            Bitmap b = BitmapConverter.ToBitmap(textmat);
            TesseractEngine ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default);
            Page texts = ocr.Process(b);
            String text = texts.GetText().Replace("\n", " ").Trim();
            Debug.WriteLine("text : " + text);
            return text;
        }

        private static void FindItemName()
        {
            Item item = new Item();
            Mat ScreenMat = BitmapConverter.ToMat(fullimage).CvtColor(ColorConversionCodes.BGRA2BGR);
            Mat rac_img = ScreenMat.InRange(new Scalar(90, 89, 82), new Scalar(90, 89, 82));
            OpenCvSharp.Point[][] contours;
            rac_img.FindContours(out contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            Mat imageOutP = new Mat(new OpenCvSharp.Size(ScreenMat.Width, ScreenMat.Height), MatType.CV_8UC3, Scalar.All(0));
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                OpenCvSharp.Rect rect2 = Cv2.BoundingRect(contour);
                if (rect2.Width > 5 && rect2.Height > 10)
                {
                    Cv2.Rectangle(imageOutP, rect2, Scalar.Green, 2);
                    String text = getTesseract(ScreenMat.SubMat(rect2));
                    if (!text.Equals(""))
                    {
                        item = FindItemInfo(text.ToCharArray());
                        break;
                    }
                }
            }
            overlay.ShowInfo(item, point);
            getItemUsage(new String(item.name));
        }

        private static void getItemList()
        {
            String[] textValue = null;
            if (File.Exists(@"Resources\itemlist.txt"))
            {
                textValue = File.ReadAllLines(@"Resources\itemlist.txt");
            }
            if (textValue != null && textValue.Length > 0)
            {
                for (int i = 2; i < textValue.Length; i++)//ignore 1,2 Line
                {
                    String[] spl = textValue[i].Split('\t');
                    Item item = new Item();
                    item.name = spl[1].Split('(')[0].Trim().ToCharArray();
                    item.price = Convert.ToInt32(spl[2]);
                    item.trader = spl[5];
                    item.trader_price = Convert.ToInt32(spl[6]);
                    item.currency = spl[7];
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }

        private static Item FindItemInfo(char[] itemname)
        {
            Item result = new Item();
            int d = 999;
            foreach (Item item in itemlist)
            {
                int d2 = levenshteinDistance(itemname, item.name);
                if (d2 < d)
                {
                    result = item;
                    d = d2;
                    if (d == 0)
                    {
                        break;
                    }
                }
            }
            Debug.WriteLine("text match : " + new String(result.name));
            return result;
        }

        private static int getMinimum(int val1, int val2, int val3)
        {
            int minNumber = val1;
            if (minNumber > val2) minNumber = val2;
            if (minNumber > val3) minNumber = val3;
            return minNumber;
        }

        private static int levenshteinDistance(char[] s, char[] t)
        {
            int m = s.Length;
            int n = t.Length;

            int[,] d = new int[m + 1, n + 1];

            for (int i = 1; i < m; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 1; j < n; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j < n; j++)
            {
                for (int i = 1; i < m; i++)
                {
                    if (s[i] == t[j])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = getMinimum(d[i - 1, j], d[i, j - 1], d[i - 1, j - 1]) + 1;
                    }
                }
            }
            return d[m - 1, n - 1];
        }

        private static void getItemUsage(String name)
        {
            StringBuilder result = new StringBuilder();
            if (!name.Equals(""))
            {
                try
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(wc.DownloadString(wiki + name));
                    HtmlAgilityPack.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//li");
                    if (nodes != null)
                    {
                        foreach (HtmlAgilityPack.HtmlNode node in nodes)
                        {
                            if (node.InnerText.Contains(" to be found "))
                            {
                                result.Append(node.InnerText).Append("\n");
                            }
                        }
                    }
                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            } else
            {
                result.Append("Item Name Not Found");
            }
            overlay.VisibleUsage(result.ToString().Trim());
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                TrayIcon.Visible = true;
                this.Hide();
                e.Cancel = true;
            } else
            {
                Application.Exit();
            }
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                TrayIcon.Visible = true;
                this.Hide();
            }
        }

        private void TrayExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void TrayShow_Click(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
