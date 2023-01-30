using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc callback, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO Dummy);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;

        private static int repeatCount = 0;
        public static System.Timers.Timer timer = new System.Timers.Timer(250);

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

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        private static readonly int WH_KEYBOARD_LL = 13;
        private static readonly int WM_KEYUP = 0x101;
        private static readonly int WH_MOUSE_LL = 14;
        private static readonly int WM_MOUSEMOVE = 0x200;
        private static bool isinfoclosed = true;
        private static bool iscompareclosed = true;
        private static LowLevelProc _proc_keyboard = null;
        private static LowLevelProc _proc_mouse = null;
        private static IntPtr hhook_keyboard = IntPtr.Zero;
        private static IntPtr hhook_mouse = IntPtr.Zero;
        private static IntPtr h_instance = LoadLibrary("User32");
        private static System.Drawing.Point point = new System.Drawing.Point(0, 0);
        private static int nFlags = 0x0;
        private static Overlay overlay_info = new Overlay(true);
        private static Overlay overlay_compare = new Overlay(false);
        private static CancellationTokenSource cts_info = new CancellationTokenSource();
        private static CancellationTokenSource cts_compare = new CancellationTokenSource();
        private static Control press_key_control = null;
        private static Scalar linecolor = new Scalar(90, 89, 82);
        private static long idle_time = 3600000;
        public static DateTime KeyPressedTime;
        private static DateTime presstime;
        public static bool WaitingForTooltip = false;

        public MainForm()
        {
            int style = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, style | WS_EX_LAYERED);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                nFlags = 0x2;
            }
            InitializeComponent();
            SettingUI();
            SetHook();
            overlay_info.Owner = this;
            overlay_info.Show();
            overlay_compare.Owner = this;
            overlay_compare.Show();

            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (repeatCount >= 12)
            {
                timer.Stop(); WaitingForTooltip = false;
                repeatCount = 0;
            }
            else
            {
                repeatCount++;

                point = Control.MousePosition;
                LoadingItemInfo();
            }
        }

        private void SettingUI()
        {
            MinimizeBox = false;
            MaximizeBox = false;
            Version.Text = Program.settings["Version"];
            MinimizetoTrayWhenStartup.Checked = Convert.ToBoolean(Program.settings["MinimizetoTrayWhenStartup"]);
            CloseOverlayWhenMouseMoved.Checked = Convert.ToBoolean(Program.settings["CloseOverlayWhenMouseMoved"]);
            RandomItem.Checked = Convert.ToBoolean(Program.settings["RandomItem"]);
            last_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Last_Price"]);
            day_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Day_Price"]);
            week_price_box.Checked = Convert.ToBoolean(Program.settings["Show_Week_Price"]);
            sell_to_trader_box.Checked = Convert.ToBoolean(Program.settings["Sell_to_Trader"]);
            buy_from_trader_box.Checked = Convert.ToBoolean(Program.settings["Buy_From_Trader"]);
            needs_box.Checked = Convert.ToBoolean(Program.settings["Needs"]);
            barters_and_crafts_box.Checked = Convert.ToBoolean(Program.settings["Barters_and_Crafts"]);
            ShowOverlay_Button.Text = ((Keys)Int32.Parse(Program.settings["ShowOverlay_Key"])).ToString();
            HideOverlay_Button.Text = ((Keys)Int32.Parse(Program.settings["HideOverlay_Key"])).ToString();
            CompareOverlay_Button.Text = ((Keys)Int32.Parse(Program.settings["CompareOverlay_Key"])).ToString();
            TransParent_Bar.Value = Int32.Parse(Program.settings["Overlay_Transparent"]);
            TransParent_Text.Text = Program.settings["Overlay_Transparent"];

            TrayIcon.Visible = true;
            check_idle_time.Start();
        }

        private void MainForm_load(object sender, EventArgs e)
        {
            //not use
        }
        private void SetHook()
        {
            SetHook(false);
        }

        private void SetHook(bool force)
        {
            try
            {
                if (force)
                {
                    Debug.WriteLine("force unhook.");
                    UnHook();
                }
                if (hhook_keyboard == IntPtr.Zero)
                {
                    _proc_keyboard = hookKeyboardProc;
                    hhook_keyboard = SetWindowsHookEx(WH_KEYBOARD_LL, _proc_keyboard, h_instance, 0);
                }
                if (Convert.ToBoolean(Program.settings["CloseOverlayWhenMouseMoved"]))
                {
                    setMouseHook();
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void setMouseHook()
        {
            if (hhook_mouse == IntPtr.Zero)
            {
                _proc_mouse = hookMouseProc;
                hhook_mouse = SetWindowsHookEx(WH_MOUSE_LL, _proc_mouse, h_instance, 0);
            }
        }

        private void unsetMouseHook()
        {
            if (hhook_mouse != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hhook_mouse);
                hhook_mouse = IntPtr.Zero;
                _proc_mouse = null;
            }
        }

        private void UnHook()
        {
            try
            {
                if (hhook_keyboard != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(hhook_keyboard);
                    hhook_keyboard = IntPtr.Zero;
                    _proc_keyboard = null;
                }
                unsetMouseHook();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private IntPtr hookKeyboardProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code >= 0 && wParam == (IntPtr)WM_KEYUP)
                {
                    if (press_key_control == null)
                    {
                        if (Program.finishloadingballistics)
                        {
                            int vkCode = Marshal.ReadInt32(lParam);
                            if (vkCode == Int32.Parse(Program.settings["ShowOverlay_Key"]))
                            {
                                KeyPressedTime = DateTime.Now;
                                Debug.WriteLine(Program.settings["ShowOverlay_Key"] + " Key Pressed.");
                                if ((!timer.Enabled || !WaitingForTooltip) && (KeyPressedTime - presstime).TotalMilliseconds >= 200)
                                {
                                    point = Control.MousePosition;
                                    LoadingItemInfo();
                                }
                                else if ((KeyPressedTime - presstime).TotalMilliseconds < 200)
                                {
                                    Debug.WriteLine("Key pressed in less than 200 milliseconds.");
                                }
                                    presstime = KeyPressedTime;
                            }
                            else if (vkCode == Int32.Parse(Program.settings["CompareOverlay_Key"]))
                            {
                                point = Control.MousePosition;
                                LoadingItemCompare();
                            }
                            else if (vkCode == Int32.Parse(Program.settings["HideOverlay_Key"])
                                || vkCode == 9 //tab
                                || vkCode == 27 //esc
                                )
                            {
                                CloseItemInfo();
                                CloseItemCompare();
                            }
                        } 
                        else
                        {
                            point = Control.MousePosition;
                            overlay_info.ShowWaitBallistics(point);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return CallNextHookEx(hhook_keyboard, code, (int)wParam, lParam);
        }

        private IntPtr hookMouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (!isinfoclosed && code >= 0
                    && wParam == (IntPtr)WM_MOUSEMOVE
                    && (Math.Abs(Control.MousePosition.X - point.X) > 60 || Math.Abs(Control.MousePosition.Y - point.Y) > 60))
                {
                    timer.Stop(); WaitingForTooltip = false; repeatCount = 0;
                    CloseItemInfo();
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return CallNextHookEx(hhook_mouse, code, (int)wParam, lParam);
        }

        private uint GetIdleTime()
        {
            LASTINPUTINFO LastUserAction = new LASTINPUTINFO();
            LastUserAction.cbSize = (uint)Marshal.SizeOf(LastUserAction);
            GetLastInputInfo(ref LastUserAction);
            return ((uint)Environment.TickCount - LastUserAction.dwTime);
        }

        public long GetTickCount()
        {
            return Environment.TickCount;
        }

        private void CloseApp()
        {
            UnHook();
            TrayIcon.Dispose();
            CloseItemInfo();
            CloseItemCompare();
            Program.SaveSettings();
            System.Windows.Forms.Application.Exit();
        }

        public void LoadingItemInfo()
        {
            isinfoclosed = false;
            cts_info.Cancel();
            cts_info = new CancellationTokenSource();

            if(timer.Enabled || WaitingForTooltip)
                overlay_info.ShowWaitinfForTooltipInfo(point, cts_info.Token);
            else
                overlay_info.ShowLoadingInfo(point, cts_info.Token);

            Task task = Task.Factory.StartNew(() => FindItemTask(true, cts_info.Token));
        }

        public void LoadingItemCompare()
        {
            if (iscompareclosed)
            {
                iscompareclosed = false;
                cts_compare.Cancel();
                cts_compare = new CancellationTokenSource();
            }
            overlay_compare.ShowLoadingCompare(point, cts_compare.Token);
            Task task = Task.Factory.StartNew(() => FindItemTask(false, cts_compare.Token));
        }

        public void CloseItemInfo()
        {
            isinfoclosed = true;
            cts_info.Cancel();
            overlay_info.HideInfo();
        }

        public void CloseItemCompare()
        {
            iscompareclosed = true;
            cts_compare.Cancel();
            overlay_compare.HideCompare();
        }

        private int FindItemTask(bool isiteminfo, CancellationToken cts_one)
        {
            if (Convert.ToBoolean(Program.settings["RandomItem"]))
            {
                if (!cts_one.IsCancellationRequested)
                {
                    Item item = Program.itemlist[new Random().Next(Program.itemlist.Count - 1)];
                    //item = MatchItemName("7.62x54r_7n37".ToLower().Trim().ToCharArray());
                    FindItemInfo(item, isiteminfo, cts_one);
                }
            } 
            else
            {
                Bitmap fullimage = CaptureScreen(CheckisTarkov());
                if (fullimage != null)
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        FindItem(fullimage, isiteminfo, cts_one);
                    }
                }
                else
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        if (isiteminfo)
                        {
                            overlay_info.ShowInfo(null, cts_one);
                        }
                        else
                        {
                            overlay_compare.ShowCompare(null, cts_one);
                        }
                    }
                    Debug.WriteLine("image null");
                }
            }
            return 0;
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

        private void ShowtestImage(Mat mat)
        {
            ShowtestImage("test", mat);
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
                using (Bitmap temp = BitmapConverter.ToBitmap(textmat))
                using (Page texts = ocr.Process(temp))
                {
                    text = texts.GetText().Replace("\n", " ").Split(Program.splitcur)[0].Trim();
                    Debug.WriteLine("text : " + text);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("tesseract error " + e.Message);
            }
            return text;
        }

        private void FindItem(Bitmap fullimage, bool isiteminfo, CancellationToken cts_one)
        {
            Item item = new Item();
            using (Mat ScreenMat_original = BitmapConverter.ToMat(fullimage))
            using (Mat ScreenMat = ScreenMat_original.CvtColor(ColorConversionCodes.BGRA2BGR))
            using (Mat rac_img = ScreenMat.InRange(linecolor, linecolor))
            {
                OpenCvSharp.Point[][] contours;
                rac_img.FindContours(out contours, out HierarchyIndex[] hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                foreach (OpenCvSharp.Point[] contour in contours)
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        OpenCvSharp.Rect rect2 = Cv2.BoundingRect(contour);
                        if (rect2.Width > 5 && rect2.Height > 10)
                        {
                            ScreenMat.Rectangle(rect2, Scalar.Black, 2);
                            using (Mat temp = ScreenMat.SubMat(rect2))
                            using (Mat temp2 = temp.Threshold(0, 255, ThresholdTypes.BinaryInv))
                            {
                                String text = getTesseract(temp2);

                                if (!text.Equals("")) //If tooltip text found
                                {
                                    item = MatchItemName(text.ToLower().Trim().ToCharArray());
                                    break;
                                }
                            }
                        }
                        else
                            timer.Start(); WaitingForTooltip = true;
                    }
                }
                
                if (!cts_one.IsCancellationRequested)
                {
                    FindItemInfo(item, isiteminfo, cts_one);
                }
            }
            fullimage.Dispose();
        }

        private Item MatchItemName(char[] itemname)
        {
            Item result = new Item();
            int d = 999;
            foreach (Item item in Program.itemlist)
            {
                int d2 = levenshteinDistance(itemname, item.name_compare);
                if (d2 < d)
                {
                    result = item;
                    d = d2;
                    if (item.isname2)
                    {
                        item.isname2 = false;
                    }
                    if (d == 0)
                    {
                        break;
                    }
                }
                d2 = levenshteinDistance(itemname, item.name_compare2);
                if (d2 < d)
                {
                    result = item;
                    d = d2;
                    item.isname2 = true;
                    if (d == 0)
                    {
                        break;
                    }
                }
            }
            if (result.name_display != null || result.name_display2 != null)
            {
                timer.Stop(); WaitingForTooltip = false; repeatCount = 0;
            }
            Debug.WriteLine(d + " text match : " + result.name_display + " & " + result.name_display2);
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

        private void FindItemInfo(Item item, bool isiteminfo, CancellationToken cts_one)
        {
            if (item.market_address != null)
            {
                try
                {
                    if (item.last_fetch == null || (DateTime.Now - item.last_fetch).TotalHours >= 1)
                    {
                        using (TPVWebClient wc = new TPVWebClient())
                        {
                            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                            Debug.WriteLine(Program.tarkovmarket + item.market_address);
                            doc.LoadHtml(wc.DownloadString(Program.tarkovmarket + item.market_address));
                            HtmlAgilityPack.HtmlNode node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='market-data']");
                            HtmlAgilityPack.HtmlNode sub_node_tm = null;
                            HtmlAgilityPack.HtmlNode sub_node_tm2 = null;
                            HtmlAgilityPack.HtmlNodeCollection nodes = null;
                            HtmlAgilityPack.HtmlNodeCollection subnodes = null;
                            HtmlAgilityPack.HtmlNodeCollection subnodes2 = null;
                            if (node_tm != null)
                            {
                                nodes = node_tm.SelectNodes(".//div[@class='w-25']");
                                sub_node_tm = node_tm.SelectSingleNode(".//div[@class='sub']");
                                if (sub_node_tm != null)
                                {
                                    item.last_updated = sub_node_tm.InnerText.Trim();
                                }
                                if (nodes != null)
                                {
                                    foreach (HtmlAgilityPack.HtmlNode node in nodes)
                                    {
                                        sub_node_tm = node.FirstChild;
                                        if (sub_node_tm != null)
                                        {
                                            if (sub_node_tm.InnerText.Trim().Contains("Flea price"))
                                            {
                                                sub_node_tm = node.SelectSingleNode(".//div[@class='big bold alt']");
                                                if (sub_node_tm != null)
                                                {
                                                    item.price_last = sub_node_tm.InnerText.Trim();
                                                }
                                            }
                                            else if (sub_node_tm.InnerText.Trim().Contains("Fee"))
                                            {
                                                sub_node_tm = node.SelectSingleNode(".//div[@class='bold alt']");
                                                if (sub_node_tm != null)
                                                {
                                                    item.fee = sub_node_tm.InnerText.Trim();
                                                }
                                            }
                                            else if (sub_node_tm.InnerText.Trim().Contains("Average price"))
                                            {
                                                subnodes = node.SelectNodes(".//span[@class='bold alt']");
                                                if (subnodes != null && subnodes.Count >= 2)
                                                {
                                                    item.price_day = subnodes[0].InnerText.Trim();
                                                    item.price_week = subnodes[1].InnerText.Trim();
                                                }
                                            }
                                            else if (sub_node_tm.InnerText.Trim().Contains("Buy from trader"))
                                            {
                                                if (node.ChildNodes.Count > 2)
                                                {
                                                    String[] temp = node.ChildNodes[2].InnerText.Trim().Split(' ');
                                                    item.buy_from_trader = temp[0] + " " + temp[1];
                                                    item.buy_from_trader_price = temp[2];
                                                }
                                            }
                                            else if (sub_node_tm.InnerText.Trim().Contains("Sell to trader"))
                                            {
                                                if (node.ChildNodes.Count > 2)
                                                {
                                                    String[] temp = node.ChildNodes[2].InnerText.Trim().Split(' ');
                                                    item.sell_to_trader = temp[0] + " " + temp[1];
                                                    item.sell_to_trader_price = temp[2];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (isiteminfo)
                            {
                                overlay_info.ShowInfo(item, cts_one);
                            }
                            else
                            {
                                overlay_compare.ShowCompare(item, cts_one);
                            }
                            bool isdisconnected = false;
                            do
                            {
                                isdisconnected = false;
                                try
                                {
                                    Debug.WriteLine(Program.wiki + item.wiki_address);
                                    try
                                    {
                                        doc.LoadHtml(wc.DownloadString(Program.wiki + item.wiki_address));
                                    }
                                    catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
                                    {
                                        Debug.WriteLine(ex.Message);
                                        Debug.WriteLine(Program.wiki + item.name_display2.Replace(" ", "_"));
                                        doc.LoadHtml(wc.DownloadString(Program.wiki + item.name_display2.Replace(" ", "_")));
                                    }
                                    node_tm = doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']");
                                    if (node_tm != null)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        nodes = node_tm.SelectNodes(".//li");
                                        if (nodes != null)
                                        {
                                            foreach (HtmlAgilityPack.HtmlNode node in nodes)
                                            {
                                                if (node.InnerText.Contains(" to be found "))
                                                {
                                                    sb.Append(node.InnerText).Append("\n");
                                                }
                                            }
                                        }
                                        sub_node_tm = node_tm.SelectSingleNode(".//td[@class='va-infobox-cont']");
                                        if (sub_node_tm != null)
                                        {
                                            nodes = sub_node_tm.SelectNodes(".//table[@class='va-infobox-group']");
                                            if (nodes != null)
                                            {
                                                foreach (HtmlAgilityPack.HtmlNode node in nodes)
                                                {
                                                    HtmlAgilityPack.HtmlNode temp_node = node.SelectSingleNode(".//th[@class='va-infobox-header']");
                                                    if (temp_node != null)
                                                    {
                                                        if (temp_node.InnerText.Trim().Equals("General data"))
                                                        {
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                            if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                            {
                                                                for (int n = 0; n < temp_node_list.Count; n++)
                                                                {
                                                                    if (temp_node_list[n].InnerText.Trim().Contains("Type"))
                                                                    {
                                                                        item.type = temp_node_list2[n].InnerText.Trim();
                                                                        if (Program.BEType.Contains(item.type))
                                                                        {
                                                                            if (!Program.blist.TryGetValue(item.name_display, out item.ballistic))
                                                                            {
                                                                                Program.blist.TryGetValue(item.name_display2, out item.ballistic);
                                                                            }
                                                                        }
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (temp_node.InnerText.Trim().Equals("Performance"))
                                                        {
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                            if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                            {
                                                                for (int n = 0; n < temp_node_list.Count; n++)
                                                                {
                                                                    if (temp_node_list[n].InnerText.Trim().Contains("Recoil"))
                                                                    {
                                                                        item.recoil = temp_node_list2[n].InnerText.Trim();
                                                                    }
                                                                    else if (temp_node_list[n].InnerText.Trim().Contains("Ergonomics"))
                                                                    {
                                                                        item.ergo = temp_node_list2[n].InnerText.Trim();
                                                                    }
                                                                    else if (temp_node_list[n].InnerText.Trim().Contains("Accuracy"))
                                                                    {
                                                                        item.accuracy = temp_node_list2[n].InnerText.Trim();
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (temp_node.InnerText.Trim().Equals("Ammunition"))
                                                        {
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list = sub_node_tm.SelectNodes(".//td[@class='va-infobox-label']");
                                                            HtmlAgilityPack.HtmlNodeCollection temp_node_list2 = sub_node_tm.SelectNodes(".//td[@class='va-infobox-content']");
                                                            if (temp_node_list != null && temp_node_list2 != null && temp_node_list.Count == temp_node_list2.Count)
                                                            {
                                                                for (int n = 0; n < temp_node_list.Count; n++)
                                                                {
                                                                    if (temp_node_list[n].InnerText.Trim().Contains("Default ammo"))
                                                                    {
                                                                        Program.blist.TryGetValue(temp_node_list2[n].InnerText.Trim(), out item.ballistic);
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        nodes = node_tm.SelectNodes(".//table[@class='wikitable']");
                                        if (nodes != null)
                                        {
                                            StringBuilder craftsb = new StringBuilder();
                                            foreach (HtmlAgilityPack.HtmlNode node in nodes)
                                            {
                                                subnodes = node.SelectNodes(".//tr");
                                                if (subnodes != null)
                                                {
                                                    foreach (HtmlAgilityPack.HtmlNode node2 in subnodes)
                                                    {
                                                        subnodes2 = node2.SelectNodes(".//th");
                                                        if (subnodes2 != null && subnodes2.Count >= 5)
                                                        {
                                                            List<String> craftlist = new List<string>();
                                                            foreach (HtmlAgilityPack.HtmlNode temp in subnodes2)
                                                            {
                                                                foreach (HtmlAgilityPack.HtmlNode temp2 in temp.ChildNodes)
                                                                {
                                                                    if (!temp2.InnerText.Trim().Equals(""))
                                                                    {
                                                                        craftlist.Add(temp2.InnerText.Trim());
                                                                    }
                                                                }
                                                            }
                                                            int firstarrow = craftlist.IndexOf("→");
                                                            int secondarrow = craftlist.LastIndexOf("→");
                                                            List<String> firstlist = craftlist.GetRange(0, firstarrow);
                                                            List<String> secondlist = craftlist.GetRange(firstarrow + 1, secondarrow - firstarrow - 1);
                                                            List<String> thirdlist = craftlist.GetRange(secondarrow + 1, craftlist.Count - secondarrow - 1);
                                                            firstlist.Reverse();
                                                            if (secondlist.Count <= 2)
                                                            {
                                                                secondlist.Reverse();
                                                            }
                                                            thirdlist.Reverse();
                                                            craftsb.Append(String.Format("{0} → {2} ({1})"
                                                                , String.Join(" ", firstlist), String.Join(secondlist.Count <= 2 ? " in " : " ", secondlist), String.Join(" ", thirdlist))).Append("\n");
                                                        }
                                                    }
                                                }
                                            }
                                            if (!craftsb.ToString().Trim().Equals(""))
                                            {
                                                item.bartersandcrafts = craftsb.ToString().Trim();
                                            }
                                        }
                                        if (!sb.ToString().Trim().Equals(""))
                                        {
                                            item.needs = sb.ToString().Trim();
                                        }
                                    }
                                }
                                catch (WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
                                {
                                    isdisconnected = true;
                                    Debug.WriteLine("wiki reconnected...");
                                }
                            } while (!cts_one.IsCancellationRequested && isdisconnected);
                        }
                    }
                    item.last_fetch = DateTime.Now;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            if (isiteminfo)
            {
                overlay_info.ShowInfo(item, cts_one);
            }
            else
            {
                overlay_compare.ShowCompare(item, cts_one);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseApp();
        }

        private void TrayExit_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void TrayShow_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//must be checked
            {
                TrayIcon.Visible = true;
                this.Hide();
                e.Cancel = true;
            }
        }

        private void MinimizetoTrayWhenStartup_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["MinimizetoTrayWhenStartup"] = (sender as CheckBox).Checked.ToString();
        }

        private void Tarkov_Official_Click(object sender, EventArgs e)
        {
            Process.Start(Program.official);
        }

        private void TarkovWiki_Click(object sender, EventArgs e)
        {
            Process.Start(Program.wiki);
        }

        private void TarkovMarket_Click(object sender, EventArgs e)
        {
            Process.Start(Program.tarkovmarket);
        }

        private void CloseOverlayWhenMouseMoved_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["CloseOverlayWhenMouseMoved"] = (sender as CheckBox).Checked.ToString();
            if ((sender as CheckBox).Checked)
            {
                setMouseHook();
            } else
            {
                unsetMouseHook();
            }
        }

        public void ChangePressKeyData(Keys? keycode)
        {
            if (press_key_control != null)
            {
                if (keycode != null)
                {
                    press_key_control.Text = keycode.ToString();
                }
                press_key_control = null;
            }
        }

        private void Overlay_Button_Click(object sender, EventArgs e)
        {
            press_key_control = (sender as Control);
            int selected = 0;
            if (press_key_control == ShowOverlay_Button)
            {
                selected = 1;
            } else if (press_key_control == HideOverlay_Button)
            {
                selected = 2;
            }
            else if (press_key_control == CompareOverlay_Button)
            {
                selected = 3;
            }
            if (selected != 0)
            {
                KeyPressCheck kpc = new KeyPressCheck(selected);
                kpc.ShowDialog(this);
            }
        }

        private void TransParent_Bar_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (sender as TrackBar);
            Program.settings["Overlay_Transparent"] = tb.Value.ToString();
            TransParent_Text.Text = Program.settings["Overlay_Transparent"] + "%";
            overlay_info.ChangeTransparent(tb.Value);
        }

        private void Github_Click(object sender, EventArgs e)
        {
            Process.Start(Program.github);
        }

        private void CheckUpdate_Click(object sender, EventArgs e)
        {
            (sender as Control).Enabled = false;
            Task task = Task.Factory.StartNew(() => UpdateTask(sender as Control));
        }

        private int UpdateTask(Control control)
        {
            try
            {
                using (TPVWebClient wc = new TPVWebClient())
                {
                    String check = wc.DownloadString(Program.checkupdate);
                    if (!check.Equals(""))
                    {
                        String sp = check.Split('\n')[0];
                        if (sp.Contains("Tarkov Price Viewer"))
                        {
                            String[] sp2 = sp.Split(' ');
                            sp = sp2[sp2.Length - 1].Trim();
                            if (!Program.settings["Version"].Equals(sp))
                            {
                                MessageBox.Show("New version (" + sp + ") found. Please download new version. Current Version is " + Program.settings["Version"]);
                                Process.Start(Program.github);
                            }
                            else
                            {
                                MessageBox.Show("Current version is newest.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Can not check update. Please check your network.");
            }
            Action show = delegate ()
            {
                control.Enabled = true;
            };
            Invoke(show);
            return 0;
        }

        private void last_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Last_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void day_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Day_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void week_price_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Show_Week_Price"] = (sender as CheckBox).Checked.ToString();
        }

        private void sell_to_trader_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Sell_to_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void buy_from_trader_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Buy_From_Trader"] = (sender as CheckBox).Checked.ToString();
        }

        private void needs_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Needs"] = (sender as CheckBox).Checked.ToString();
        }

        private void barters_and_crafts_box_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["Barters_and_Crafts"] = (sender as CheckBox).Checked.ToString();
        }

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void RandomItem_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["RandomItem"] = (sender as CheckBox).Checked.ToString();
        }

        private void check_idle_time_Tick(object sender, EventArgs e)
        {
            if (GetIdleTime() >= idle_time)
            {
                idle_time += 3600000;
                SetHook(true);
            } else
            {
                if (idle_time > 3600000)
                {
                    idle_time = 3600000;
                }
                SetHook();
            }
        }

        private void refresh_b_Click(object sender, EventArgs e)
        {
            SetHook(true);
        }
    }
}
