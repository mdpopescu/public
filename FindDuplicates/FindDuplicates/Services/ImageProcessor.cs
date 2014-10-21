using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FindDuplicates.Services
{
  public class ImageProcessor
  {
    public Image Minify(Image image)
    {
      const int WIDTH = 4;
      const int HEIGHT = 4;

      var destRect = new Rectangle(0, 0, WIDTH, HEIGHT);
      var destImage = new Bitmap(WIDTH, HEIGHT);

      using (var g = Graphics.FromImage(destImage))
      {
        g.CompositingMode = CompositingMode.SourceCopy;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var wrapMode = new ImageAttributes())
        {
          wrapMode.SetWrapMode(WrapMode.TileFlipXY);
          g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        }
      }

      return destImage;
    }

    public byte[] GetBytes(Image image)
    {
      byte[] rawImageData;
      lock (image)
      {
        var converter = new ImageConverter();
        rawImageData = converter.ConvertTo(image, typeof (byte[])) as byte[];
      }

      Debug.Assert(rawImageData != null, "rawImageData != null");
      return rawImageData;
    }
  }
}