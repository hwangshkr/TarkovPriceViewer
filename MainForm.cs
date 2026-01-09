using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Online;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using static TarkovPriceViewer.TarkovAPI;

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
        private static readonly int WM_LBUTTONUP = 0x0202;
        private static readonly int WM_RBUTTONUP = 0x0205;
        private static readonly int WM_MBUTTONUP = 0x0208;
        private static readonly int WM_XBUTTONUP = 0x020C;
        private const int MOUSE_LEFT = 1001;
        private const int MOUSE_RIGHT = 1002;
        private const int MOUSE_MIDDLE = 1003;
        private const int MOUSE_X1 = 1004;
        private const int MOUSE_X2 = 1005;
        private const int XBUTTON1 = 0x0001;
        private const int XBUTTON2 = 0x0002;
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
        private static object lockObject = new object();
        public static RecognizationModel languageModel = null;
        private static PaddleOcrRecognizer ocrRecognizer = null;
        private static readonly object ocrLock = new object();
        public static DateTime KeyPressedTime;
        private static DateTime presstime;
        public static bool WaitingForTooltip = false;
        public static bool GettingItemInfo = false;

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
            if (!GettingItemInfo)
            {
                if (repeatCount >= 0)//why was 12?
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
        }

        private void PaddleRecognizer(Action<PaddleOcrRecognizer> action)
        {
            EnsureRecognizer();
            if (ocrRecognizer == null)
            {
                return;
            }

            lock (ocrLock)
            {
                action?.Invoke(ocrRecognizer);
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
            ShowOverlay_Button.Text = GetKeybindText(Program.settings["ShowOverlay_Key"]);
            HideOverlay_Button.Text = GetKeybindText(Program.settings["HideOverlay_Key"]);
            CompareOverlay_Button.Text = GetKeybindText(Program.settings["CompareOverlay_Key"]);
            TransParent_Bar.Value = Int32.Parse(Program.settings["Overlay_Transparent"]);
            TransParent_Text.Text = Program.settings["Overlay_Transparent"];
            TarkovTrackerCheckBox.Checked = Convert.ToBoolean(Program.settings["useTarkovTrackerAPI"]);
            hideoutUpgrades_checkBox.Checked = Convert.ToBoolean(Program.settings["showHideoutUpgrades"]);
            tarkovTrackerApiKey_textbox.Text = Program.settings["TarkovTrackerAPIKey"];
            if (decimal.TryParse(Program.settings[Program.WorthPerSlotThresholdKey], NumberStyles.Any, CultureInfo.InvariantCulture, out var worthThreshold))
            {
                worthThreshold = Math.Max(worthThresholdNumeric.Minimum, Math.Min(worthThresholdNumeric.Maximum, worthThreshold));
                worthThresholdNumeric.Value = worthThreshold;
            }
            else
            {
                worthThresholdNumeric.Value = (decimal)Program.WorthPerSlotThresholdDefault;
            }

            languageBox.Items.Add("en");
            languageBox.Items.Add("ko");
            languageBox.Items.Add("cn");
            languageBox.Items.Add("jp");
            languageBox.Items.Add("ru");
            languageBox.DropDownStyle = ComboBoxStyle.DropDownList;
            languageBox.SelectedItem = Program.settings["Language"];
            languageBox.SelectedIndexChanged += languageBox_SelectedIndexChanged;

            modeBox.Items.Add("regular");
            modeBox.Items.Add("pve");
            modeBox.DropDownStyle = ComboBoxStyle.DropDownList;
            modeBox.SelectedItem = Program.settings["Mode"];
            modeBox.SelectedIndexChanged += modeBox_SelectedIndexChanged;

            PaddleRecognizer(null);//init ocr

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
                setMouseHook();
            }
            catch (Exception e)
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
                            if ((Program.finishloadingTarkovTrackerAPI && Convert.ToBoolean(Program.settings["useTarkovTrackerAPI"])) || !Convert.ToBoolean(Program.settings["useTarkovTrackerAPI"]))
                            {
                                int vkCode = Marshal.ReadInt32(lParam);
                                HandleGlobalKeyOrMouse(vkCode);
                            }
                            else
                            {
                                point = Control.MousePosition;
                                overlay_info.ShowWaitAPI(point);
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

        private void HandleGlobalKeyOrMouse(int code)
        {
            try
            {
                int showKey = Int32.Parse(Program.settings["ShowOverlay_Key"]);
                int compareKey = Int32.Parse(Program.settings["CompareOverlay_Key"]);
                int hideKey = Int32.Parse(Program.settings["HideOverlay_Key"]);

                if (code == showKey)
                {
                    KeyPressedTime = DateTime.Now;
                    Debug.WriteLine("\n\n----------------" + Program.settings["ShowOverlay_Key"] + " Key Pressed -----------------");
                    if ((!timer.Enabled || !WaitingForTooltip) && (KeyPressedTime - presstime).TotalMilliseconds >= 200)
                    {
                        point = Control.MousePosition;
                        WaitingForTooltip = true; timer.Start();
                        LoadingItemInfo();
                    }
                    else if ((KeyPressedTime - presstime).TotalMilliseconds < 200)
                    {
                        Debug.WriteLine("Key pressed in less than 200 milliseconds.");
                    }
                    presstime = KeyPressedTime;
                }
                else if (code == compareKey)
                {
                    point = Control.MousePosition;
                    LoadingItemCompare();
                }
                else if (code == hideKey
                    || code == 9 //tab
                    || code == 27 //esc
                    )
                {
                    CloseItemInfo();
                    CloseItemCompare();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private IntPtr hookMouseProc(int code, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (code >= 0)
                {
                    if (!isinfoclosed
                        && Convert.ToBoolean(Program.settings["CloseOverlayWhenMouseMoved"]) 
                        && wParam == (IntPtr)WM_MOUSEMOVE
                        && (Math.Abs(Control.MousePosition.X - point.X) > 60 || Math.Abs(Control.MousePosition.Y - point.Y) > 60))
                    {
                        timer.Stop(); WaitingForTooltip = false; repeatCount = 0;
                        CloseItemInfo();
                        Task UpdateAPI = Task.Factory.StartNew(() => Program.UpdateItemListAPI());
                        Task UpdateTarkovTrackerAPI = Task.Factory.StartNew(() => Program.UpdateTarkovTrackerAPI());
                    }

                    if (press_key_control == null)
                    {
                        int mouseCode = 0;

                        if (wParam == (IntPtr)WM_LBUTTONUP)
                        {
                            mouseCode = MOUSE_LEFT;
                        }
                        else if (wParam == (IntPtr)WM_RBUTTONUP)
                        {
                            mouseCode = MOUSE_RIGHT;
                        }
                        else if (wParam == (IntPtr)WM_MBUTTONUP)
                        {
                            mouseCode = MOUSE_MIDDLE;
                        }
                        else if (wParam == (IntPtr)WM_XBUTTONUP)
                        {
                            int mouseData = Marshal.ReadInt32(lParam, 8);
                            int buttonFlag = (mouseData >> 16) & 0xffff;
                            if (buttonFlag == XBUTTON1)
                            {
                                mouseCode = MOUSE_X1;
                            }
                            else if (buttonFlag == XBUTTON2)
                            {
                                mouseCode = MOUSE_X2;
                            }
                        }

                        if (mouseCode != 0)
                        {
                            HandleGlobalKeyOrMouse(mouseCode);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return CallNextHookEx(hhook_mouse, code, (int)wParam, lParam);
        }

        private string GetKeybindText(string value)
        {
            int code;
            if (!int.TryParse(value, out code))
            {
                return value;
            }

            switch (code)
            {
                case MOUSE_LEFT:
                    return "Mouse Left";
                case MOUSE_RIGHT:
                    return "Mouse Right";
                case MOUSE_MIDDLE:
                    return "Mouse Middle";
                case MOUSE_X1:
                    return "Mouse X1";
                case MOUSE_X2:
                    return "Mouse X2";
                default:
                    return ((Keys)code).ToString();
            }
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
            if (TarkovTrackerCheckBox.Checked)
                Program.settings["TarkovTrackerAPIKey"] = tarkovTrackerApiKey_textbox.Text;

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

            if (timer.Enabled || WaitingForTooltip)
                overlay_info.ShowWaitingForTooltipInfo(point, cts_info.Token);
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
                    try
                    {
                        Item item = Program.tarkovAPI.items[new Random().Next(Program.tarkovAPI.items.Count - 1)];
                        //item = MatchItemName("7.62x54r_7n37".ToLower().Trim().ToCharArray());
                        FindItemInfoAPI(item, isiteminfo, cts_one);
                    }
                    catch
                    {
                        Debug.WriteLine("itemlist null");
                    }
                }
            }
            else
            {
                Bitmap fullimage = CaptureScreen(CheckisTarkov());
                if (fullimage != null)
                {
                    if (!cts_one.IsCancellationRequested)
                    {
                        FindItemAPI(fullimage, isiteminfo, cts_one);
                    }
                }
                else
                {
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

        private void getPaddleModel()
        {
            Task getModel = Task.Run(async () => {
                Debug.WriteLine("Download the paddle language model.");
                RecognizationModel model;
                if (Program.settings["Language"] == "ko")
                {
                    model = await LocalDictOnlineRecognizationModel.KoreanV4.DownloadAsync();
                }
                else if (Program.settings["Language"] == "cn")
                {
                    model = await LocalDictOnlineRecognizationModel.ChineseV4.DownloadAsync();
                }
                else if (Program.settings["Language"] == "jp")
                {
                    model = await LocalDictOnlineRecognizationModel.JapanV4.DownloadAsync();
                }
                else if (Program.settings["Language"] == "ru")
                {
                    model = await LocalDictOnlineRecognizationModel.CyrillicV3.DownloadAsync();
                }
                else
                {
                    model = await LocalDictOnlineRecognizationModel.EnglishV4.DownloadAsync();
                }

                lock (lockObject)
                {
                    Debug.WriteLine("language model setted.");
                    languageModel = model;
                }
            });
        }

        private void EnsureRecognizer()
        {
            getPaddleModel();
            if (languageModel == null)
            {
                return;
            }

            if (ocrRecognizer == null)
            {
                lock (ocrLock)
                {
                    if (ocrRecognizer == null)
                    {
                        try
                        {
                            ocrRecognizer = new PaddleOcrRecognizer(languageModel, PaddleDevice.Gpu());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error creating PaddleOcrRecognizer: " + e.Message);
                            try
                            {
                                ocrRecognizer = new PaddleOcrRecognizer(languageModel, PaddleDevice.Mkldnn());
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Error creating CPU PaddleOcrRecognizer: " + ex.Message);
                                ocrRecognizer = null;
                            }
                        }
                    }
                }
            }
        }

        private String getPaddleOCR(Mat textmat)
        {
            GettingItemInfo = true;
            String text = "";
            try
            {
                EnsureRecognizer();
                if (ocrRecognizer == null)
                {
                    GettingItemInfo = false;
                    return text;
                }

                lock (ocrLock)
                {
                    var result = ocrRecognizer.Run(textmat);
                    if (result.Score > 0.5f)
                    {
                        text = result.Text.Replace("\n", " ").Split(Program.splitcur)[0].Trim();
                    }
                    Debug.WriteLine(result.Score + " Paddle Text : " + result.Text);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Paddle error: " + e.Message);
                GettingItemInfo = false;
            }
            GettingItemInfo = false;
            return text;
        }

        private void FindItemAPI(Bitmap fullimage, bool isiteminfo, CancellationToken cts_one)
        {
            int a = 1;
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
                                String text = getPaddleOCR(temp);
                                String text2 = getPaddleOCR(temp2);

                                if (!text.Equals("") || !text2.Equals("")) //If tooltip text found
                                {
                                    item = MatchItemNameAPI(text, text2);
                                    break;
                                }
                            }
                        }
                        /*else
                            timer.Start(); WaitingForTooltip = true;*/
                    }
                }

                if (!cts_one.IsCancellationRequested)
                {
                    FindItemInfoAPI(item, isiteminfo, cts_one);
                }
            }
            fullimage.Dispose();
        }

        private Item MatchItemNameAPI(string name, string name2)
        {
            char[] itemname = name.ToLower().Trim().ToCharArray();
            char[] itemname2 = name2.ToLower().Trim().ToCharArray();

            var result = new Item();
            if (Program.tarkovAPI == null)
            {
                Debug.WriteLine("error : no item list.");
                return result;
            }
            int d = 999;
            foreach (var item in Program.tarkovAPI.items)
            {
                int d2;
                if (itemname.Length > 0)
                {
                    d2 = levenshteinDistance(itemname, item.name.ToLower().ToCharArray());
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

                if (itemname2.Length > 0)
                {
                    d2 = levenshteinDistance(itemname2, item.name.ToLower().ToCharArray());
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
            }
            if (result.name != null)
            {
                timer.Stop(); WaitingForTooltip = false; repeatCount = 0;
            }
            if (name == "Encrypted message" || name2 == "Encrypted message")
            {
                result = new Item();
                result.name = "Encrypted Message";
            }
            Debug.WriteLine(d + " text match : " + result.name);
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

        private void FindItemInfoAPI(Item item, bool isiteminfo, CancellationToken cts_one)
        {
            if (item.link != null) //Market link
            {
                try
                {
                    if (item.types.Exists(e => e.Equals("ammo")) && !item.types.Exists(e => e.Equals("grenade"))) // "Round", "Slug", "Buckshot", "Grenade launcher cartridge" / ("gun" and not "preset" = weapons)
                    {
                        Program.blist.TryGetValue(item.name, out item.ballistic);
                    }

                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            if (isiteminfo)
            {
                overlay_info.ShowInfoAPI(item, cts_one);
            }
            else
            {
                overlay_compare.ShowCompareAPI(item, cts_one);
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
            }
            else
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
                else
                {
                    ShowOverlay_Button.Text = GetKeybindText(Program.settings["ShowOverlay_Key"]);
                    HideOverlay_Button.Text = GetKeybindText(Program.settings["HideOverlay_Key"]);
                    CompareOverlay_Button.Text = GetKeybindText(Program.settings["CompareOverlay_Key"]);
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
            }
            else if (press_key_control == HideOverlay_Button)
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
            System.Windows.Forms.TrackBar tb = (sender as System.Windows.Forms.TrackBar);
            Program.settings["Overlay_Transparent"] = tb.Value.ToString();
            TransParent_Text.Text = Program.settings["Overlay_Transparent"] + "%";
            overlay_info.ChangeTransparent(tb.Value);
        }

        private void Github1_Click(object sender, EventArgs e)
        {
            Process.Start(Program.github[0]);
        }
        private void Github2_Click(object sender, EventArgs e)
        {
            Process.Start(Program.github[1]);
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
                    string highgithub = "";
                    float highv = 0;
                    foreach (var github in Program.github)
                    {
                        String check = wc.DownloadString(github + Program.checkupdate);
                        if (!check.Equals(""))
                        {
                            String sp = check.Split('\n')[0];
                            if (sp.Contains("Tarkov Price Viewer"))
                            {
                                String[] sp2 = sp.Split(' ');
                                sp = sp2[sp2.Length - 1].Trim();
                                float v;
                                if (float.TryParse(sp.Replace("v", ""), out v))
                                {
                                    if (highv < v)
                                    {
                                        highgithub = github;
                                        highv = v;
                                    }
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(highgithub))
                    {
                        string vs = "v" + highv;
                        if (!Program.settings["Version"].Equals(vs))
                        {
                            MessageBox.Show("New version (" + vs + ") found. \nCurrent Version is " + Program.settings["Version"]);
                            Process.Start(highgithub);
                        }
                        else
                        {
                            MessageBox.Show("You already have the latest version.");
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

        private void worthThresholdNumeric_ValueChanged(object sender, EventArgs e)
        {
            Program.settings[Program.WorthPerSlotThresholdKey] = worthThresholdNumeric.Value.ToString(CultureInfo.InvariantCulture);
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
            }
            else
            {
                if (idle_time > 3600000)
                {
                    idle_time = 3600000;
                }
                SetHook();
            }
        }

        private void refreshAPI_b_Click(object sender, EventArgs e)
        {
            Program.settings["TarkovTrackerAPIKey"] = tarkovTrackerApiKey_textbox.Text;
            Task.Factory.StartNew(() => Program.UpdateTarkovTrackerAPI());

            SetHook(true);
        }

        private void Tarkov_Dev_Click(object sender, EventArgs e)
        {
            Process.Start(Program.tarkov_dev);
        }

        private void TarkovTrackerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["useTarkovTrackerAPI"] = (sender as CheckBox).Checked.ToString();
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            TarkovTrackerAPI_tooltip.SetToolTip(label1, "Use TarkovTracker.io to show only your active tasks and hideout upgrades. And hide completed.");
        }

        private void TarkovTrackerCheckBox_MouseHover(object sender, EventArgs e)
        {
            TarkovTrackerAPI_tooltip.SetToolTip(TarkovTrackerCheckBox, "Use TarkovTracker.io to show only your active tasks and hideout upgrades. And hide completed.");
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            TarkovTrackerAPI_tooltip.SetToolTip(label2, "Go to \"https://tarkovtracker.io/settings\" to get your API key.");
        }

        private void tarkovTrackerApiKey_textbox_MouseHover(object sender, EventArgs e)
        {
            TarkovTrackerAPI_tooltip.SetToolTip(tarkovTrackerApiKey_textbox, "Go to \"https://tarkovtracker.io/settings\" to get your API key.");
        }

        private void TarkovTracker_button_Click(object sender, EventArgs e)
        {
            Process.Start(Program.tarkovtracker);
        }

        private void hideoutUpgrades_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Program.settings["showHideoutUpgrades"] = (sender as CheckBox).Checked.ToString();
        }

        private void languageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as System.Windows.Forms.ComboBox).SelectedIndex >= 0)
            {
                Program.settings["Language"] = (sender as System.Windows.Forms.ComboBox).SelectedItem.ToString();
            }
            else
            {
                Program.settings["Language"] = "en";
            }
            lock (lockObject)
            {
                languageModel = null;
            }
            PaddleRecognizer(null);//init ocr

            Program.forceUpdateAPI = true;
            Program.forceUpdateTrackerAPI = true;
        }

        private void modeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as System.Windows.Forms.ComboBox).SelectedIndex >= 0)
            {
                Program.settings["Mode"] = (sender as System.Windows.Forms.ComboBox).SelectedItem.ToString();
            }
            else
            {
                Program.settings["Mode"] = "regular";
            }
            Program.forceUpdateAPI = true;
            Program.forceUpdateTrackerAPI = true;
        }
    }
}
