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
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;
using Tesseract;

namespace TarkovPriceViewer
{
    public partial class MainForm : Form
    {

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

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

        private static readonly int WH_KEYBOARD_LL = 13;
        private static readonly int WM_KEYDOWN = 0x100;
        private static readonly Overlay overlay = new Overlay();
        private static readonly String setting_path = @"settings.json";
        private static readonly String appname = "EscapeFromTarkov";
        private static readonly String wiki = "https://escapefromtarkov.fandom.com/wiki/";
        private static readonly String tarkovmarket = "https://tarkov-market.com/item/";
        private static readonly List<Item> itemlist = new List<Item>();
        private static readonly WebClient wc = new WebClient();
        private static LowLevelKeyboardProc _proc = null;
        private static Dictionary<String, String> settings;
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
            if (Environment.OSVersion.Version.Major == 10)
            {
                nFlags = 0x2;
            }
            LoadSettings();
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
            _proc = hookProc;
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, hInstance, 0);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public IntPtr hookProc(int code, IntPtr wParam, IntPtr lParam)
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
                        backthread = new Thread(FindItemThread);
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

        private void LoadSettings()
        {
            try
            {
                if (!File.Exists(setting_path))
                {
                    File.Create(setting_path);
                }
                String text = File.ReadAllText(setting_path);
                settings = JsonSerializer.Deserialize<Dictionary<String, String>>(text);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void CloseItemInfo()
        {
            if (backthread != null)
            {
                backthread.Abort();
            }
            overlay.setItemInfoVisible(false);
        }

        private void FindItemThread()
        {
            CaptureScreen(CheckisTarkov());
            if (fullimage != null)
            {
                FindItem();
            }
            else
            {
                Debug.WriteLine("image null");
            }
        }

        private IntPtr CheckisTarkov()
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd != IntPtr.Zero)
            {
                StringBuilder sbWinText = new StringBuilder(260);
                GetWindowText(hWnd, sbWinText, 260);
                if (sbWinText.ToString() == appname)
                {
                    return hWnd;
                }
            }
            Debug.WriteLine("error - no app");
            return IntPtr.Zero;
        }

        private void CaptureScreen(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                Graphics Graphicsdata = Graphics.FromHwnd(hWnd);
                Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(hWnd, hdc, nFlags);
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
                }
                catch (Exception e)
                {
                    Debug.WriteLine("no test img" + e.Message);
                }
#endif
            }
        }

        private void ShowtestImage(String name, Mat mat)
        {
            Action show = delegate ()
            {
                Cv2.ImShow(name, mat);
            };
            Invoke(show);
        }

        private String getTesseract(Mat textmat)
        {
            String text = "";
            try
            {
                Bitmap b = BitmapConverter.ToBitmap(textmat);
                TesseractEngine ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default);//should use once
                Page texts = ocr.Process(b);
                text = texts.GetText().Replace("\n", " ").Trim();
                Debug.WriteLine("text : " + text);
            }
            catch (Exception e)
            {
                Debug.WriteLine("tesseract error " + e.Message);
            }
            return text;
        }

        private void FindItem()
        {
            Item item = new Item();
            Mat ScreenMat = BitmapConverter.ToMat(fullimage).CvtColor(ColorConversionCodes.BGRA2BGR);
            Mat rac_img = ScreenMat.InRange(new Scalar(90, 89, 82), new Scalar(90, 89, 82));
            OpenCvSharp.Point[][] contours;
            rac_img.FindContours(out contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                OpenCvSharp.Rect rect2 = Cv2.BoundingRect(contour);
                if (rect2.Width > 5 && rect2.Height > 10)
                {
                    ScreenMat.Rectangle(rect2, Scalar.Black, 2);
                    String text = getTesseract(ScreenMat.SubMat(rect2));
                    if (!text.Equals(""))
                    {
                        item = MatchItemName(text.Trim().ToCharArray());
                    }
                    break;
                }
            }
            FindItemInfo(item);
            overlay.ShowInfo(item, point);
        }

        private void getItemList()
        {
            String[] textValue = null;
            if (File.Exists(@"Resources\itemlist.txt"))
            {
                textValue = File.ReadAllLines(@"Resources\itemlist.txt");
            }
            if (textValue != null && textValue.Length > 0)
            {
                for (int i = 0; i < textValue.Length; i++)//ignore 1,2 Line
                {
                    String[] spl = textValue[i].Split('\t');
                    Item item = new Item();
                    item.name = spl[0].Trim().ToCharArray();//for compare '_' removed
                    item.name_tm = spl[1].Trim();//for address '_' not removed
                    itemlist.Add(item);
                }
            }
            Debug.WriteLine("itemlist Count : " + itemlist.Count);
        }

        private Item MatchItemName(char[] itemname)
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

        private int getMinimum(int val1, int val2, int val3)
        {
            int minNumber = val1;
            if (minNumber > val2) minNumber = val2;
            if (minNumber > val3) minNumber = val3;
            return minNumber;
        }

        private int levenshteinDistance(char[] s, char[] t)
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

        private void FindItemInfo(Item item)
        {
            if (item.name_tm != null)
            {
                try
                {
                    if (item.last_updated == null)
                    {
                        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(wc.DownloadString(tarkovmarket + item.name_tm));
                        HtmlAgilityPack.HtmlNode node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='updated-block']");
                        if (node_tm != null)
                        {
                            item.last_updated = node_tm.InnerText.Trim();
                        }
                        node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='c-price last alt']");
                        if (node_tm != null)
                        {
                            item.price = node_tm.InnerText.Trim();
                        }
                        node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='prices-blk']");
                        if (node_tm != null)
                        {
                            HtmlAgilityPack.HtmlNode node2 = node_tm.SelectSingleNode("//div[@class='bold']");
                            if (node2 != null)
                            {
                                item.trader = node2.InnerText.Trim();
                            }
                            node2 = node_tm.SelectSingleNode("//div[@class='c-price alt']");
                            if (node2 != null)
                            {
                                item.trader_price = node2.InnerText.Trim();
                            }
                        }
                        doc.LoadHtml(wc.DownloadString(wiki + item.name_tm));
                        HtmlAgilityPack.HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//li");
                        if (nodes != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (HtmlAgilityPack.HtmlNode node in nodes)
                            {
                                if (node.InnerText.Contains(" to be found "))
                                {
                                    sb.Append(node.InnerText).Append("\n");
                                }
                            }
                            item.Needs = sb.ToString().Trim();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                TrayIcon.Visible = true;
                this.Hide();
                e.Cancel = true;
            }
            else
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
