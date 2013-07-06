using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace Renfield.QRMaker
{
  public class Generator
  {
    public Generator(QrEncoder encoder)
    {
      this.encoder = encoder;
    }

    public void Render(string data, Stream stream)
    {
      var qrCode = encoder.Encode(data);
      var renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);
      renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, stream);
    }

    //

    private readonly QrEncoder encoder;
  }
}