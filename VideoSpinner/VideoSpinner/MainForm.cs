using System;
using System.Collections.Generic;
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

    private void SetStatus(string status)
    {
      lblStatus.Text = status;
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
        Width = 640,
        Height = 480,
        WatermarkFile = txtWatermarkFile.Text,
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

    private void ProcessRecords(IEnumerable<string[]> data, VideoSpec spec, VideoMaker maker)
    {
      foreach (var record in data)
      {
        spec.Name = Path.Combine(txtOutputFolder.Text, record[0] + ".wmv");
        spec.Text = record[1];

        SetStatus(spec.Name);

        pbStatus.Increment(1);
        maker.Create(spec);
        pbStatus.Increment(1);
      }
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
      using (var dlg = new OpenFileDialog
      {
        Filter = "Image Files (*.gif;*.jpg;*.png)|*.gif;*.jpg;*.png|All files (*.*)|*.*"
      })
      {
        if (dlg.ShowDialog() == DialogResult.OK)
          txtWatermarkFile.Text = dlg.FileName;
      }
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
        var logger = new FileLogger(Path.Combine(txtOutputFolder.Text, "log.txt"));
        var maker = new WmvVideoMaker(Path.GetTempPath(), new RandomShuffler(), logger);
        var spec = CreateSpec();

        var data = ReadCsv(txtCsvFile.Text).ToList();
        pbStatus.Maximum = data.Count * 2;
        pbStatus.Value = 0;

        ProcessRecords(data, spec, maker);
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings.Default.Save();
    }
  }
}