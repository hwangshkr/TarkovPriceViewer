using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TarkovPriceViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void testB_Click(object sender, EventArgs e)
        {
            Bitmap image = ScreenShot.Capture();
            if (image != null)
            {
                image.Save("C:\\Users\\DreamChain\\Desktop\\test.png", ImageFormat.Png);
            } else
            {
                Debug.WriteLine("image null");
            }
        }
    }
}
