using System;
using System.Windows.Forms;
using TechnicalDrawing.Library.Contracts;
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

        private static string GetFilename()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                return dlg.ShowDialog() == DialogResult.OK ? dlg.FileName : null;
            }
        }

        //

        private readonly FileSystem fs = new WinFileSystem();
        private readonly Parser parser = new Parser();
        private readonly Projector projector = new Projector();

        private void MainForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void FileOpenMenu_Click(object sender, EventArgs e)
        {
            var app = new DrawingApp(fs, parser, projector, () => new ImageCanvas(XYImage, XZImage, YZImage));
            app.OpenFile(GetFilename);
        }
    }
}