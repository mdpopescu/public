using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace FindDuplicates.Services
{
  public class ImageProcessor
  {
    // partially based on http://stackoverflow.com/questions/10964179/colormatrix-for-partial-gray-scale
    public Bitmap MinifyAndGrayscale(Image image)
    {
      var destRect = new Rectangle(0, 0, WIDTH, HEIGHT);
      var minified = new Bitmap(WIDTH, HEIGHT, PixelFormat.Format32bppRgb);

      using (var g = Graphics.FromImage(minified))
      {
        g.CompositingMode = CompositingMode.SourceCopy;
        g.CompositingQuality = CompositingQuality.HighQuality;
        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var attributes = new ImageAttributes())
        {
          attributes.SetWrapMode(WrapMode.TileFlipXY);
          attributes.SetColorMatrix(matrix);

          g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

          return minified;
        }
      }
    }

    // based on http://www.hackerfactor.com/blog/index.php?/archives/529-Kind-of-Like-That.html
    public ulong GetDiffHash(Bitmap minified)
    {
      lock (minified)
      {
        ulong result = 0;

        // the input bitmap is 9x8 and grayscale
        for (var y = 0; y < HEIGHT; y++)
        {
          // start x from 1 because I'll compare [x-1] to [x]
          for (var x = 1; x < WIDTH; x++)
          {
            result = (result << 1) | (uint) (minified.GetPixel(x - 1, y).ToArgb() < minified.GetPixel(x, y).ToArgb() ? 0 : 1);
          }
        }

        return result;
      }
    }

    //

    private const int WIDTH = 9;
    private const int HEIGHT = 8;

    private static readonly ColorMatrix matrix = new ColorMatrix(new[]
    {
      new[] { .3f, .3f, .3f, 0, 0 },
      new[] { .59f, .59f, .59f, 0, 0 },
      new[] { .11f, .11f, .11f, 0, 0 },
      new float[] { 0, 0, 0, 1, 0 },
      new float[] { 0, 0, 0, 0, 1 }
    });
  }
}