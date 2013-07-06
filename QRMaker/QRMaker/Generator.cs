using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Renfield.QRMaker
{
  public class Generator
  {
    public QrSettings Settings { get; set; }

    public Generator(QrEncoder encoder)
    {
      Settings = new QrSettings();
      this.encoder = encoder;
    }

    public void Render(string data, string fileName)
    {
      encoder.ErrorCorrectionLevel = Settings.ErrorCorrectionLevel;

      var qrCode = encoder.Encode(data);
      var renderer = new GraphicsRenderer(new FixedCodeSize(Settings.Size, QuietZoneModules.Zero), Settings.Background, Settings.Foreground);

      using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, file);
    }

    //

    private readonly QrEncoder encoder;
  }
}