using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

namespace TarkovPriceViewer
{
    static class ScreenShot
    {

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        private const String appname = "notepad";

        public static Bitmap Capture()
        {
            Bitmap image = null;
            IntPtr app = FindWindow(appname, null);
            if (app != IntPtr.Zero)
            {
                Graphics gfxWin = Graphics.FromHwnd(app);
                Rectangle rc = Rectangle.Round(gfxWin.VisibleClipBounds);
                image = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(image);
                IntPtr hdcBitmap = gfxBmp.GetHdc();
                bool succeeded = PrintWindow(app, hdcBitmap, 1);
                gfxBmp.ReleaseHdc(hdcBitmap);
                if (!succeeded)
                {
                    gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, image.Size));
                }
                IntPtr hRgn = CreateRectRgn(0, 0, 0, 0);
                GetWindowRgn(app, hRgn);
                Region region = Region.FromHrgn(hRgn);
                if (!region.IsEmpty(gfxBmp))
                {
                    gfxBmp.ExcludeClip(region);
                    gfxBmp.Clear(Color.Transparent);
                }
                gfxBmp.Dispose();
            } else
            {
                Debug.WriteLine("error" + app);
            }
            return image;
        }
    }
}
