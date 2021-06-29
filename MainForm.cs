using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
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
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        internal static extern bool GetWindowPlacement(int hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(int hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(int hwnd, int id);

        internal struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public ShowWindowCommands showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        internal enum ShowWindowCommands : int
        {
            Hide = 0,
            Normal = 1,
            Minimized = 2,
            Maximized = 3,
        }

        private int nFlags = 0x0;
        private const String appname = "EscapeFromTarkov";
        private Bitmap _fullimage = new Bitmap(@"img\test.png");
        public Bitmap fullimage { get { return _fullimage; } }
        private Overlay overlay = new Overlay();
        private Thread backthread = null;
        private System.Drawing.Point point = new System.Drawing.Point(0, 0);

        public MainForm()
        {
            InitializeComponent();
            overlay.Show();
        }

        private void MainForm_load(object sender, EventArgs e)
        {
            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;
            if (v.Major == 10)
            {
                nFlags = 0x2;
            }
            RegisterHotKey((int)this.Handle, 0, 0x0, (int)Keys.F9);
        }

        private void MainForm_closed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey((int)this.Handle, 0);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)0x312)
            {
                if (m.WParam == (IntPtr)0x0)
                {
                    var cursor = Control.MousePosition;
                    point = new System.Drawing.Point(cursor.X, cursor.Y);
                    overlay.setItemInfoVisible(false);
                    if (backthread != null)
                    {
                        backthread.Abort();
                    }
                    backthread = new Thread(BackWork);
                    backthread.IsBackground = true;
                    backthread.Start();
                }
            }
        }

        private void BackWork()
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

        private bool CheckisTarkov()
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

        private void CaptureScreen()
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
                _fullimage = bmp;
            }
            else
            {
                Debug.WriteLine("error - no window");
            }
        }

        private void CheckTass(Mat textmat)
        {
            var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.LstmOnly);
            Bitmap b = BitmapConverter.ToBitmap(textmat);
            var texts = ocr.Process(b);
            String text = texts.GetText().Replace("\n", " ").Trim();
            Debug.WriteLine("text : " + text);
            if (!text.Equals(""))
            {
                overlay.ShowInfo(text, point);
            }
        }

        private void FindItemName()
        {
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
                    CheckTass(ScreenMat.SubMat(rect2));
                }
            }
        }
    }
}
