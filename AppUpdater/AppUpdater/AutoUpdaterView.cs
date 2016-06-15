using System.Drawing;

namespace Renfield.AppUpdater
{
  public interface AutoUpdaterView
  {
    string ManifestURI { get; set; }
    bool IsRightAnchored { get; }

    void SetImage(Image image);
    void SetToolTip(string caption);
    void SetCheckVisible(bool show);
    void SetUpdateVisible(bool show);
    void SetProgressVisible(bool show);
    void SetProgressValue(int percentage);
    void Enlarge();
    void Shrink();
    void MoveLeft();
    void MoveRight();
  }
}