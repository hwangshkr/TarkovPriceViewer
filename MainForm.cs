using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
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
        private static readonly WebClient wc = new WebClient();
        private static readonly HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
        private static LowLevelKeyboardProc _proc = null;
        private static IntPtr hhook = IntPtr.Zero;
        private static int nFlags = 0x0;
        private static Thread backthread = null;
        private static Overlay overlay = new Overlay();
        private static long presstime = 0;

        public MainForm()
        {
            InitializeComponent();
            var style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
            if (Environment.OSVersion.Version.Major == 10)
            {
                nFlags = 0x2;
            }
            MaximizeBox = false;
            TrayIcon.Visible = true;
            HideFormWhenStartup();
            SettingWebClient();
            SetHook();
            overlay.Show();
        }

        private void MainForm_load(object sender, EventArgs e)
        {
            //not use
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
                        long CurrentTime = DateTime.Now.Ticks;
                        if (CurrentTime - presstime > 10000000)
                        {
                            LoadingItemInfo(Control.MousePosition);
                            backthread = new Thread(FindItemThread);
                            backthread.IsBackground = true;
                            backthread.Start();
                        }
                        else
                        {
                            Debug.WriteLine("key pressed in 1 second.");
                        }
                        presstime = CurrentTime;
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

        private void SettingWebClient()
        {
            doc.DisableServerSideCode = true;
            doc.OptionDefaultStreamEncoding = Encoding.UTF8;
            doc.OptionUseIdAttribute = false;
            wc.Proxy = null;
            wc.Encoding = Encoding.UTF8;
        }

        public void LoadingItemInfo(System.Drawing.Point point)
        {
            if (backthread != null)
            {
                backthread.Abort();
                backthread.Join();
            }
            overlay.ShowLoadingInfo(point);
        }

        public void CloseItemInfo()
        {
            if (backthread != null)
            {
                backthread.Abort();
                backthread.Join();
            }
            overlay.HideInfo();
        }

        private void FindItemThread()
        {
            using (Bitmap fullimage = CaptureScreen(CheckisTarkov()))
            {
                if (fullimage != null)
                {
                    FindItem(fullimage);
                }
                else
                {
                    Debug.WriteLine("image null");
                }
            }
        }

        private IntPtr CheckisTarkov()
        {
            IntPtr hWnd = GetForegroundWindow();
            if (hWnd != IntPtr.Zero)
            {
                StringBuilder sbWinText = new StringBuilder(260);
                GetWindowText(hWnd, sbWinText, 260);
                if (sbWinText.ToString() == Program.appname)
                {
                    return hWnd;
                }
            }
            Debug.WriteLine("error - no app");
            return IntPtr.Zero;
        }

        private Bitmap CaptureScreen(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                using (Graphics Graphicsdata = Graphics.FromHwnd(hWnd))
                {
                    Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);
                    Bitmap bmp = new Bitmap(rect.Width, rect.Height);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        IntPtr hdc = g.GetHdc();
                        PrintWindow(hWnd, hdc, nFlags);
                        g.ReleaseHdc(hdc);
                    }
                    return bmp;
                }
            }
            else
            {
#if DEBUG
                try
                {
                    return new Bitmap(@"img\test.png");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("no test img" + e.Message);
                }
#endif
                Debug.WriteLine("error - no window");
                return null;
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
                using (TesseractEngine ocr = new TesseractEngine(@"./Resources/tessdata", "eng", EngineMode.Default))//should use once
                using (Page texts = ocr.Process(BitmapConverter.ToBitmap(textmat)))
                {
                    text = texts.GetText().Replace("\n", " ").Trim();
                    Debug.WriteLine("text : " + text);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("tesseract error " + e.Message);
            }
            return text;
        }

        private void FindItem(Bitmap fullimage)
        {
            Item item = new Item();
            using (Mat ScreenMat = BitmapConverter.ToMat(fullimage).CvtColor(ColorConversionCodes.BGRA2BGR))
            using (Mat rac_img = ScreenMat.InRange(new Scalar(90, 89, 82), new Scalar(90, 89, 82)))
            {
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
                            break;
                        }
                    }
                }
                FindItemInfo(item);
                overlay.ShowInfo(item);
            }
        }

        private Item MatchItemName(char[] itemname)
        {
            Item result = new Item();
            int d = 999;
            foreach (Item item in Program.itemlist)
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
                        doc.LoadHtml(wc.DownloadString(Program.tarkovmarket + item.name_tm));
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
                        node_tm = null;
                        doc.LoadHtml(wc.DownloadString(Program.wiki + item.name_tm));
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
                        nodes = null;
                        doc.LoadHtml("");
                        wc.Dispose();
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
            Application.Exit();
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

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }
    }
}
