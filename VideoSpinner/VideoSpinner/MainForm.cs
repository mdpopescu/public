using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Renfield.VideoSpinner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private void SetColor(Color color)
        {
            color = Color.FromArgb((int) (color.ToArgb() | 0xff000000));

            colorStrip.BackColor = color;
            txtColor.Text = string.Format("#{0:x6}", color.ToArgb() & 0xffffff);
        }

        //

        private void btnBrowse1_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog {Filter = "Csv Files|*.csv|All files|*.*"})
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtCsvFile.Text = dlg.FileName;
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            var dlg = new FolderSelectDialog();
            if (dlg.ShowDialog())
                txtImages.Text = dlg.FileName;
        }

        private void btnBrowse3_Click(object sender, EventArgs e)
        {
            var dlg = new FolderSelectDialog();
            if (dlg.ShowDialog())
                txtSounds.Text = dlg.FileName;
        }

        private void btnBrowse4_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.Cancel)
                return;

            var color = colorDialog.Color;
            SetColor(color);
        }

        private void btnBrowse5_Click(object sender, EventArgs e)
        {
            var dlg = new FolderSelectDialog();
            if (dlg.ShowDialog())
                txtOutputFolder.Text = dlg.FileName;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //
        }

        private void txtWatermarkSize_Leave(object sender, EventArgs e)
        {
            int size;
            if (!int.TryParse(txtWatermarkSize.Text, out size))
                size = 24;
            if (size < 8)
                size = 8;
            if (size > 72)
                size = 72;

            txtWatermarkSize.Text = size.ToString();
        }

        private void txtColor_Leave(object sender, EventArgs e)
        {
            Color color;

            var text = txtColor.Text;
            if (!text.StartsWith("#"))
                color = Color.FromName(text);
            else
            {
                if (text.Length == 4)
                {
                    text = string.Format("#{0}{0}{1}{1}{2}{2}", text[1], text[2], text[3]);
                    txtColor.Text = text;
                }

                int value;
                int.TryParse(text.Substring(1), NumberStyles.HexNumber, null, out value);
                color = Color.FromArgb(value);
            }

            SetColor(color);
        }
    }
}