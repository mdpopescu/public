using System.Drawing;
using Gma.QrCodeNet.Encoding;

namespace Renfield.QRMaker
{
  public class QrSettings
  {
    public ErrorCorrectionLevel ErrorCorrectionLevel { get; set; }
    public int Size { get; set; }
    public Brush Background { get; set; }
    public Brush Foreground { get; set; }

    public QrSettings()
    {
      ErrorCorrectionLevel = ErrorCorrectionLevel.M;
      Size = 200;
      Background = Brushes.Black;
      Foreground = Brushes.White;
    }
  }
}