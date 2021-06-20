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
        private int rc = 62 * 4;
        private const String appname = "notepad";
        private Bitmap _fullimage = new Bitmap(@"img\test.png");
        public Bitmap fullimage { get { return _fullimage; } }
        private Overlay overlay = new Overlay();
        private List<ItemImage> itemimagelist = new List<ItemImage>();

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

        internal struct ItemImage
        {
            public String name;
            public Mat image;
            public Mat hist;
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
            getItemImageList();
            overlay.Show();
        }

        private void getItemImageList()
        {
            DirectoryInfo di = new DirectoryInfo(@"images");
            Debug.WriteLine("loading item images.");
            foreach (FileInfo File in di.GetFiles())
            {
                if (File.Exists)
                {
                    ItemImage ii = new ItemImage();
                    ii.name = File.Name;
                    ii.image = Cv2.ImRead(File.FullName, ImreadModes.Color).Resize(new OpenCvSharp.Size(rc, rc));
                    ii.hist = CalculateHist(ii.image);
                    itemimagelist.Add(ii);
                }
            }
            Debug.WriteLine("finish loading item images : " + itemimagelist.Count);
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

        OpenCvSharp.Point[][] contours;
        Mat ScreenMat = new Mat();
        int rec_size = 0;
        Mat rac_img = new Mat();

        private bool CheckisInventory()
        {
            try
            {
                ScreenMat = BitmapConverter.ToMat(fullimage);
                //using (Mat FindMat = BitmapConverter.ToMat(checkinventoryimage))
                //using (Mat res = ScreenMat.MatchTemplate(FindMat, TemplateMatchModes.CCoeffNormed))
                {
                    double minval, maxval = 0;
                    OpenCvSharp.Point minloc, maxloc;
                    //Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                    //Debug.WriteLine("image diff : " + maxval + ", pos : " + maxloc.X + "-" + maxloc.Y + ", size : " + FindMat.Width + "-" + FindMat.Height);

                    /*Mat canny_img = new Mat();
                    Cv2.CvtColor(ScreenMat, ScreenMat, ColorConversionCodes.BGR2HSV);
                    Mat[] mv = new Mat[3];
                    mv = Cv2.Split(ScreenMat);
                    Cv2.CvtColor(ScreenMat, ScreenMat, ColorConversionCodes.HSV2BGR);
                    Cv2.InRange(mv[0], new Scalar(26), new Scalar(108), canny_img);
                    Cv2.BitwiseAnd(ScreenMat, canny_img.CvtColor(ColorConversionCodes.GRAY2BGR), canny_img);
                    //canny_img = canny_img.Canny(150, 300, 3, true);
                    testdrawbox.Image = BitmapConverter.ToBitmap(canny_img);*/

                    Mat canny_img = new Mat();
                    Cv2.InRange(ScreenMat, new Scalar(45, 45, 45), new Scalar(115, 130, 130), canny_img);
                    //Cv2.BitwiseAnd(ScreenMat, canny_img, canny_img);
                    //canny_img = canny_img.Canny(0, 255, 3, false);
                    //testdrawbox.Image = BitmapConverter.ToBitmap(canny_img);

                    //rac_img = new Mat(canny_img.Rows, canny_img.Cols, MatType.CV_8UC3);
                    Cv2.CopyTo(ScreenMat, rac_img);
                    //Cv2.CvtColor(canny_img, rac_img, ColorConversionCodes.GRAY2BGR);
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(canny_img, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                    Debug.WriteLine("contours : " + contours.Length);

                    //int s = 86;62
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

        private void next_Click(object sender, EventArgs e)
        {
            FindMatchImage();
        }

        private bool FindMatchImage()
        {
            ItemImage result = new ItemImage();
            Mat submat = new Mat();
            Mat hist1_last = new Mat();
            OpenCvSharp.Point point = new OpenCvSharp.Point(1550, 300);
            Cv2.Circle(rac_img, point.X, point.Y, 5, Scalar.Red);
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                OpenCvSharp.Point[] poly = Cv2.ApproxPolyDP(contour, 1, true);
                if (Cv2.PointPolygonTest(contour, point, false) == 1
                    && poly.Length >= 4
                    //&& GetDistance(poly[0], poly[1]) >= 31 && GetDistance(poly[1], poly[2]) >= 31 && GetDistance(poly[2], poly[3]) >= 31 && GetDistance(poly[3], poly[0]) >= 31
                    )
                {
                    rec_size++;
                    int minx = int.MaxValue, maxx = 0, miny = int.MaxValue, maxy = 0;
                    for (int j = 0; j < poly.Length; j++)
                    {
                        if (minx > poly[j].X)
                        {
                            minx = poly[j].X;
                        }
                        if (maxx < poly[j].X)
                        {
                            maxx = poly[j].X;
                        }
                        if (miny > poly[j].Y)
                        {
                            miny = poly[j].Y;
                        }
                        if (maxy < poly[j].Y)
                        {
                            maxy = poly[j].Y;
                        }
                        Cv2.Line(rac_img, poly[j], poly[(j + 1) % poly.Length], Scalar.Green, 2);
                    }
                    Debug.WriteLine("xy : " + minx + "," + miny + "," + (maxx - minx) + "," + (maxy - miny));
                    submat = ScreenMat.SubMat(new Rect(minx, miny, maxx - minx, maxy - miny));

                    double last_hist = 0;
                    double last_maxval = 0;
                    Mat match = submat.Resize(new OpenCvSharp.Size(rc, rc));
                    Mat hist1 = CalculateHist(match);

                    foreach (ItemImage item in itemimagelist)
                    {

                        double hist = Cv2.CompareHist(hist1, item.hist, HistCompMethods.Correl);
                        if (item.name.Equals("cricket_lighter_icon.png"))
                        {
                            Debug.WriteLine(" result found : " + hist);
                            Mat t3 = new Mat();
                            item.hist.ConvertTo(t3, MatType.CV_8UC1);
                            imageshould.Image = BitmapConverter.ToBitmap(t3);
                        }
                        if (hist > last_hist && hist > 0.5)
                        {
                            Mat res = match.MatchTemplate(item.image, TemplateMatchModes.CCoeffNormed);
                            double minval, maxval = 0;
                            OpenCvSharp.Point minloc, maxloc;
                            Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                            if (maxval > 0.5)
                            {
                                last_hist = hist;
                                last_maxval = maxval;
                                result = item;
                                hist1_last = hist1;
                            }
                        }
                    }
                    Debug.WriteLine(last_maxval + " result : " + last_hist + ", name : " + result.name);
                    break;
                }
            }
            Debug.WriteLine("rec size : " + rec_size);
            Mat t1 = new Mat();
            result.hist.ConvertTo(t1, MatType.CV_8UC1);
            imagefound.Image = BitmapConverter.ToBitmap(t1);
            Mat t2 = new Mat();
            hist1_last.ConvertTo(t2, MatType.CV_8UC1);
            imagesub.Image = BitmapConverter.ToBitmap(t2);
            testdrawbox.Image = BitmapConverter.ToBitmap(rac_img);
            return !result.Equals(default(ItemImage));
        }

        private int[] channels = { 0, 1 };
        private Rangef[] ranges = { new Rangef(0, 180), new Rangef(0, 256) };
        private int[] histSize = { 180, 256 };
        private Mat CalculateHist(Mat img)
        {
            Mat hist = new Mat();
            Cv2.CalcHist(new Mat[] { img.CvtColor(ColorConversionCodes.BGR2HSV) }, channels, null, hist, 2, histSize, ranges);
            //Cv2.Normalize(hist, hist, 0, 255, NormTypes.MinMax);
            return hist;
        }
    }
}
