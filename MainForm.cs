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

        internal struct ItemImage
        {
            public String name;
            public Mat image;
            public Mat hist;
        }

        private int nFlags = 0x0;
        private int rc = 62 * 4;
        private const String appname = "EscapeFromTarkov";
        private Bitmap _fullimage = new Bitmap(@"img\test.png");
        public Bitmap fullimage { get { return _fullimage; } }
        private Overlay overlay = new Overlay();
        private List<ItemImage> itemimagelist = new List<ItemImage>();

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
            getItemImageList();
            // 0x0 : 조합키 없이 사용, 0x1: ALT, 0x2: Ctrl, 0x3: Shift
            //RegisterHotKey(핸들러함수, 등록키의_ID, 조합키, 등록할_키)
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
                    Debug.WriteLine("key pressed with point : " + cursor.ToString());
                    CaptureScreen();
                    CheckisInventory();
                    FindMatchImage(new OpenCvSharp.Point(cursor.X, cursor.Y));
                }
            }
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

        OpenCvSharp.Point[][] contours;
        Mat ScreenMat = new Mat();
        Mat canny_img = new Mat();
        Mat rac_img = new Mat();

        private bool CheckisInventory()
        {
            try
            {
                ScreenMat = BitmapConverter.ToMat(fullimage).CvtColor(ColorConversionCodes.BGRA2BGR);
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

                    //Cv2.InRange(ScreenMat, new Scalar(0, 0, 0), new Scalar(255, 255, 255), canny_img);
                    //Cv2.InRange(ScreenMat, new Scalar(45, 45, 45), new Scalar(115, 130, 130), canny_img);
                    //Cv2.BitwiseAnd(ScreenMat, canny_img, canny_img);
                    //canny_img = ScreenMat.Canny(0, 255, 3, false);
                    Cv2.AdaptiveThreshold(ScreenMat.CvtColor(ColorConversionCodes.BGR2GRAY), canny_img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, 3, 12);
                    Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
                    Cv2.MorphologyEx(canny_img, canny_img, MorphTypes.Close, kernel);
                    //testdrawbox.Image = BitmapConverter.ToBitmap(canny_img);

                    //rac_img = new Mat(canny_img.Rows, canny_img.Cols, MatType.CV_8UC3);
                    Cv2.CopyTo(canny_img.CvtColor(ColorConversionCodes.GRAY2BGR), rac_img);
                    //Cv2.CvtColor(canny_img, rac_img, ColorConversionCodes.GRAY2BGR);
                    HierarchyIndex[] hierarchy;
                    Cv2.FindContours(canny_img, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);
                    Debug.WriteLine("contours : " + contours.Length);
                    testdrawbox.Image = BitmapConverter.ToBitmap(canny_img);

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

        private void CheckTass(Mat textmat)
        {
            /*var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractOnly);
            Mat test = new Mat();
            Mat test2 = textmat.SubMat(new OpenCvSharp.Rect(0, 0, textmat.Width, 14));
            OpenCvSharp.Point[][] testc;
            Cv2.AdaptiveThreshold(test2.CvtColor(ColorConversionCodes.BGR2GRAY), test, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, 3, 12);
            Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 1));
            Cv2.MorphologyEx(test, test, MorphTypes.Close, kernel);
            Bitmap b = BitmapConverter.ToBitmap(test);
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(test, out testc, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            if (testc.Length > 0)
            {
                foreach (OpenCvSharp.Point[] c in testc)
                {
                    OpenCvSharp.Rect rect = Cv2.BoundingRect(c);
                    if (rect.Height > 10)
                    {
                        b = BitmapConverter.ToBitmap(test2.SubMat(rect));
                        Cv2.Rectangle(test2, rect, Scalar.Red, 1);
                        Debug.WriteLine(rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height);
                    }
                }
            }
            var texts = ocr.Process(BitmapConverter.ToBitmap(test2));
            b = BitmapConverter.ToBitmap(test2);
            //Bitmap b = BitmapConverter.ToBitmap(textmat.SubMat(new OpenCvSharp.Rect(0, 0, textmat.Width, 14)).CvtColor(ColorConversionCodes.BGR2GRAY));
            texttest.Image = b;
            texttest2.Image = BitmapConverter.ToBitmap(test);
            Debug.WriteLine(testc.Length + " text : " + texts.GetText());*/
            var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.LstmOnly);
            Bitmap b = BitmapConverter.ToBitmap(textmat);
            var texts = ocr.Process(b);
            texttest2.Image = b;
            String text = texts.GetText().Trim();
            /*if (text.Contains("\n"))
            {
                String[] str = text.Split('\n');
                text = str[0].Trim();
            }*/
            text = text.Replace("\n", "");
            Debug.WriteLine(" text : " + text);
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
            //FindMatchImage(new OpenCvSharp.Point(850, 750));
            Mat test = ScreenMat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //Cv2.AdaptiveThreshold(ScreenMat.CvtColor(ColorConversionCodes.BGR2GRAY), canny_img, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.BinaryInv, 3, 12);
            //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(2, 2));
            //Cv2.MorphologyEx(canny_img, canny_img, MorphTypes.Close, kernel);
            Cv2.Threshold(test, test, 120, 255, ThresholdTypes.Binary);// | ThresholdTypes.Otsu
            Mat test2 = test.CvtColor(ColorConversionCodes.GRAY2BGR);
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
                if (rect.Width >= 50 && rect.Height >= 50)
                {
                    Cv2.Rectangle(test2, rect, Scalar.Green, 2);
                    CheckTass(test.SubMat(rect).SubMat(new OpenCvSharp.Rect(0, 0, rect.Width, 14)));
                }
            }
            testdrawbox.Image = BitmapConverter.ToBitmap(test2);
        }

        private bool FindMatchImage(OpenCvSharp.Point point)
        {
            ItemImage result = new ItemImage();
            Mat hist1_last = new Mat();
            Cv2.Circle(rac_img, point.X, point.Y, 5, Scalar.Red);
            List<OpenCvSharp.Rect> rectlist = new List<OpenCvSharp.Rect>();
            foreach (OpenCvSharp.Point[] contour in contours)
            {
                //OpenCvSharp.Point[] poly = Cv2.ApproxPolyDP(contour, 1, true);
                OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
                if (//poly.Length >= 4 &&
                    rect.Contains(point) &&
                    rect.Width >= 50 && rect.Height >= 50
                    )
                {
                    rectlist.Add(rect);
                    Cv2.Rectangle(rac_img, rect, Scalar.Green, 2);
                }
            }
            double last_hist = 0;
            double last_maxval = 0;
            if (rectlist.Count > 0)
            {
                rectlist.Sort((r1, r2) => (r1.Width * r1.Height).CompareTo((r2.Width * r2.Height)));
                foreach (OpenCvSharp.Rect r in rectlist)
                {
                    Mat match = ScreenMat.SubMat(r);
                    match = match.Resize(new OpenCvSharp.Size(rc, rc));
                    OpenCvSharp.Rect rect = new OpenCvSharp.Rect(1, 1, match.Width - 7, match.Height - 7);
                    Mat mask = new Mat(match.Size(), MatType.CV_8UC1);
                    Cv2.GrabCut(match, mask, rect, new Mat(), new Mat(), 5, GrabCutModes.InitWithRect);
                    var lut2 = new byte[256];
                    lut2[0] = 0; lut2[1] = 1; lut2[2] = 0; lut2[3] = 1;
                    Cv2.LUT(mask, lut2, mask);
                    var foreground = new Mat(match.Size(), MatType.CV_8UC3, new Scalar(0, 0, 0));
                    match.CopyTo(foreground, mask);
                    Cv2.ImShow("a", foreground);
                    Mat test = ScreenMat.CvtColor(ColorConversionCodes.BGR2GRAY).SubMat(r).SubMat(new OpenCvSharp.Rect(0, 0, r.Width, 14));
                    Cv2.Threshold(test, test, 0, 120, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                    Cv2.ImShow("b", test);
                    CheckTass(test);
                    Mat hist1 = CalculateHist(match);
                    Debug.WriteLine((r.Width * r.Height));

                    foreach (ItemImage item in itemimagelist)
                    {
                        double hist = Cv2.CompareHist(hist1, item.hist, HistCompMethods.Correl);
                        if (item.name.Equals("cricket_lighter_icon.png"))
                        {
                            Mat res = match.MatchTemplate(item.image, TemplateMatchModes.CCoeffNormed);
                            double minval, maxval = 0;
                            OpenCvSharp.Point minloc, maxloc;
                            Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
                            Debug.WriteLine(maxval + " result found : " + hist);
                            Mat t3 = new Mat();
                            item.hist.ConvertTo(t3, MatType.CV_8UC1);
                            imageshould.Image = BitmapConverter.ToBitmap(t3);
                        }
                        if (hist > last_hist)
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
                    if (!result.Equals(default(ItemImage)))
                    {
                        break;
                    }
                }
                Debug.WriteLine(last_maxval + " result : " + last_hist + ", name : " + result.name);
            }
            Debug.WriteLine("rec size : " + rectlist.Count);
            /*Mat t1 = new Mat();
            result.hist.ConvertTo(t1, MatType.CV_8UC1);
            imagefound.Image = BitmapConverter.ToBitmap(t1);
            Mat t2 = new Mat();
            hist1_last.ConvertTo(t2, MatType.CV_8UC1);
            imagesub.Image = BitmapConverter.ToBitmap(t2);*/
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
