using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Renfield.AppUpdater.Helper;

namespace Renfield.AppUpdater
{
  [ToolboxBitmap(@"S:\GIT-public\AppUpdater\AppUpdater\Images\ok.png")]
  public partial class AutoUpdater : UserControl, AutoUpdaterView
  {
    private readonly Presenter presenter;
    private readonly Updater updater;

    public AutoUpdater()
    {
      InitializeComponent();

      presenter = new AutoUpdaterPresenter(this);
      updater = new WebUpdater(presenter, () => new WebLoader());
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
      width = 16 + (progressBar.Visible ? progressBar.Width : 0);
      height = 16;

      base.SetBoundsCore(x, y, width, height, specified);
    }

    #region Implementation of AutoUpdaterView

    public string ManifestURI { get; set; }

    public bool IsRightAnchored
    {
      get { return (Anchor & AnchorStyles.Right) != 0; }
    }

    public void SetImage(Image image)
    {
      if (picture.InvokeRequired)
        picture.Invoke(new Action(() => SetImage(image)));
      else
        picture.Image = image;
    }

    public void SetToolTip(string caption)
    {
      if (picture.InvokeRequired)
        picture.Invoke(new Action(() => SetToolTip(caption)));
      else
      {
        toolTip.SetToolTip(picture, caption);
        toolTip.SetToolTip(progressBar, caption);
      }
    }

    public void SetCheckVisible(bool show)
    {
      if (contextMenu.InvokeRequired)
        contextMenu.Invoke(new Action(() => SetCheckVisible(show)));
      else
        checkToolStripMenuItem.Visible = show;
    }

    public void SetUpdateVisible(bool show)
    {
      if (contextMenu.InvokeRequired)
        contextMenu.Invoke(new Action(() => SetUpdateVisible(show)));
      else
        updateToolStripMenuItem.Visible = show;
    }

    public void SetProgressVisible(bool show)
    {
      progressBar.Visible = show;
    }

    public void SetProgressValue(int percentage)
    {
      progressBar.Value = percentage;
    }

    public void Enlarge()
    {
      Width = Width + progressBar.Width;
    }

    public void Shrink()
    {
      Width = Width - progressBar.Width;
    }

    public void MoveLeft()
    {
      Left = Left - progressBar.Width;
    }

    public void MoveRight()
    {
      Left = Left + progressBar.Width;
    }

    #endregion

    private void AutoUpdater_Load(object sender, EventArgs e)
    {
      presenter.Initialize();

      if (!DesignMode)
        updater.CheckForUpdate(ManifestURI, Application.ProductVersion, new XmlManifestReader());
    }

    private void checkToolStripMenuItem_Click(object sender, EventArgs e)
    {
      updater.CheckForUpdate(ManifestURI, Application.ProductVersion, new XmlManifestReader());
    }

    private void updateToolStripMenuItem_Click(object sender, EventArgs e)
    {
      updater.Update(Path.GetTempPath(), new ProcessManager());
    }

    private void picture_Click(object sender, EventArgs e)
    {
      var clickLocation = ((MouseEventArgs) e).Location;
      contextMenu.Show(picture, clickLocation);
    }
  }
}