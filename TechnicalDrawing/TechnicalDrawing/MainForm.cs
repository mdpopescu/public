using System;
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

        //

        private void MainForm_Load(object sender, EventArgs e)
        {
            var canvas = new ImageCanvas(XYImage, XZImage, YZImage);
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