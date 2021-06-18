using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

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

        [DllImport("user32")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32")]
        private static extern IntPtr GetActiveWindow();


        private int nFlags = 0x0;
        private const String appname = "notepad";
        private Bitmap _fullimage = new Bitmap(@"img\test.png");
        public Bitmap fullimage { get { return _fullimage; } }
        private Overlay overlay = new Overlay();

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

        public MainForm()
        {
            InitializeComponent();

            OperatingSystem os = Environment.OSVersion;
            Version v = os.Version;
            if (v.Major == 10)
            {
                nFlags = 0x2;
            }
            overlay.Show();
        }

        private void testB_Click(object sender, EventArgs e)
        {
            if (CheckisTarkov())
            {
                var cursor = Control.MousePosition;
                CaptureScreen();
                if (fullimage != null)
                {
                    //testimageview.BackgroundImage = fullimage;
                    CheckisInventory();
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
            IntPtr findwindow = FindWindow(appname, null);
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

        private bool CheckisInventory()
        {
            try
            {
                using (Mat ScreenMat = BitmapConverter.ToMat(fullimage))
                //using (Mat FindMat = BitmapConverter.ToMat(checkinventoryimage))
                //using (Mat res = ScreenMat.MatchTemplate(FindMat, TemplateMatchModes.CCoeffNormed))
                {
                    double minval, maxval = 0;
                    OpenCvSharp.Point minloc, maxloc;
                    //Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                    //Debug.WriteLine("image diff : " + maxval + ", pos : " + maxloc.X + "-" + maxloc.Y + ", size : " + FindMat.Width + "-" + FindMat.Height);

                    Mat canny_img = new Mat();
                    Cv2.CvtColor(ScreenMat, ScreenMat, ColorConversionCodes.BGR2HSV);
                    Mat[] mv = new Mat[3];
                    mv = Cv2.Split(ScreenMat);
                    Cv2.CvtColor(ScreenMat, ScreenMat, ColorConversionCodes.HSV2BGR);
                    Cv2.InRange(mv[0], new Scalar(90), new Scalar(98), canny_img);
                    Cv2.BitwiseAnd(ScreenMat, canny_img.CvtColor(ColorConversionCodes.GRAY2BGR), canny_img);
                    //canny_img = canny_img.Canny(130, 255, 3, false);
                    testdrawbox.Image = BitmapConverter.ToBitmap(canny_img);

                    /*Mat rac_img = new Mat(canny_img.Rows, canny_img.Cols, MatType.CV_8UC1);
                    Cv2.CvtColor(canny_img, rac_img, ColorConversionCodes.GRAY2BGR);
                    OpenCvSharp.Point[][] contours;
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(canny_img, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                    Debug.WriteLine("contours : " + contours.Length);
                    int rec_size = 0;
                    for (int i = 0; i < contours.Length; i++)
                    {

                        OpenCvSharp.Point[] poly = Cv2.ApproxPolyDP(contours[i], 1.2, true);
                        if (poly.Length == 4
                            && GetDistance(poly[0], poly[1]) >= 43 && GetDistance(poly[1], poly[2]) >= 43 && GetDistance(poly[2], poly[3]) >= 43 && GetDistance(poly[3], poly[0]) >= 43
                            )
                        {
                            rec_size++;
                            for (int j = 0; j < 4; j++)
                            {
                                Cv2.Line(rac_img, poly[j], poly[(j + 1) % poly.Length], rec_size % 2 == 0 ? Scalar.Red : Scalar.Green, 2);
                            }
                        }
                    }
                    Debug.WriteLine("rec size : " + rec_size);
                    testdrawbox.Image = BitmapConverter.ToBitmap(rac_img);*/


                    //int s = 86;
                    //DrawBox(7, 10, s, s);
                    if (maxval >= 0.8)
                    {
                        //DrawBox(maxloc.X * 640 / ScreenMat.Width, maxloc.Y * 480 / ScreenMat.Height, FindMat.Width * 640 / ScreenMat.Width, FindMat.Height * 480 / ScreenMat.Height);
                        return true;
                    }
                }
            } catch (Exception e)
            {
                Debug.WriteLine("error : " + e.Message);
            }
            return false;
        }

        public static Double GetDistance(OpenCvSharp.Point p1, OpenCvSharp.Point p2)
        {
            Double xdf = p2.X - p1.X;
            Double ydf = p2.Y - p1.Y;
            return Math.Sqrt(Math.Pow(xdf, 2) + Math.Pow(ydf, 2));
        }

        private void DrawBox(int x, int y, int w, int h)
        {
            Graphics g = testdrawbox.CreateGraphics();
            Pen p = new Pen(Color.Blue, 1);
            Rectangle rec = new Rectangle(x, y, w, h);
            g.DrawRectangle(p, rec);
            Debug.WriteLine("draw box");
        }
    }
}
