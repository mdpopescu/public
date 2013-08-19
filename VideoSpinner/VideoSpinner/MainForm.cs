using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DataAccess;
using Renfield.VideoSpinner.Library;
using Renfield.VideoSpinner.Properties;

namespace Renfield.VideoSpinner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //

        private Color GetColor()
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

            return color;
        }

        private void SetColor(Color color)
        {
            color = Color.FromArgb((int) (((uint) color.ToArgb()) | 0xff000000));

            colorStrip.BackColor = color;
            txtColor.Text = string.Format("#{0:x6}", color.ToArgb() & 0xffffff);
        }

        private void SetStatus(string status)
        {
            lblStatus.Text = status;
            pbStatus.Increment(1);
        }

// ReSharper disable InconsistentNaming
        private void DisableUI()
        {
            Controls
                .Cast<Control>()
                .ToList()
                .ForEach(it => it.Enabled = false);
        }

        private void EnableUI()
        {
            Controls
                .Cast<Control>()
                .ToList()
                .ForEach(it => it.Enabled = true);
        }

// ReSharper restore InconsistentNaming

        private VideoSpec CreateSpec()
        {
            return new VideoSpec
            {
                Width = 160,
                Height = 120,
                WatermarkText = txtWatermark.Text,
                WatermarkSize = int.Parse(txtWatermarkSize.Text),
                WatermarkColor = GetColor(),
                ImageFiles = Directory.GetFiles(txtImages.Text).ToList(),
                SoundFiles = Directory.GetFiles(txtSounds.Text).ToList(),
            };
        }

        private static IEnumerable<string[]> ReadCsv(string fileName)
        {
            return DataTable
                .New
                .ReadCsv(fileName)
                .Rows
                .Select(row => new[] {row["Keyword"], row["Text to speech"]})
                .ToList();
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
            Settings.Default.Save();

            using (new WaitGuard())
            using (new Guard(DisableUI, EnableUI))
            {
                var maker = new WmvVideoMaker(Path.GetTempPath(), new RandomShuffler());
                var spec = CreateSpec();

                var data = ReadCsv(txtCsvFile.Text).ToList();
                pbStatus.Maximum = data.Count;
                pbStatus.Value = 0;

                foreach (var record in data)
                {
                    spec.Name = Path.Combine(txtOutputFolder.Text, record[0] + ".wmv");
                    spec.Text = record[1];

                    SetStatus(spec.Name);
                    
                    maker.Create(spec);
                }
            }
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
            var color = GetColor();
            SetColor(color);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}