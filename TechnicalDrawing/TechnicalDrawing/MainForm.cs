using System;
using System.Drawing;
using System.Windows.Forms;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Shell;
using TechnicalDrawing.Shell;

namespace TechnicalDrawing
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private DrawingApp app;

        private static Image CreateImage(int width, int height, Color color)
        {
            var image = new Bitmap(width, height);

            using (var g = Graphics.FromImage(image))
            using (var b = new SolidBrush(color))
                g.FillRectangle(b, 0, 0, image.Width, image.Height);

            return image;
        }

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            XYImage.Image = CreateImage(XYImage.Width, XYImage.Height, Color.White);
            XZImage.Image = CreateImage(XZImage.Width, XZImage.Height, Color.White);
            YZImage.Image = CreateImage(YZImage.Width, YZImage.Height, Color.White);

            var canvas = new ImageCanvas(XYImage.Image, XZImage.Image, YZImage.Image);
            app = new DrawingApp(new WinFileSystem(), new Parser(), new Projector(), canvas);
        }

        private void FileOpenMenu_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                if (dlg.ShowDialog() == DialogResult.OK)
                    app.Load(dlg.FileName);
            }
        }
    }
}