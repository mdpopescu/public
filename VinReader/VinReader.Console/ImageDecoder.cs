using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using ZXing;

namespace Renfield.VinReader.Command
{
  public class ImageDecoder
  {
    public string Decode(Bitmap bmp)
    {
      return GenerateEffects(bmp)
        .Select(InternalDecode)
        .Where(it => it != null)
        .FirstOrDefault();
    }

    //

    private const int SLICE_SIZE = 50;

    private static IEnumerable<Bitmap> GenerateEffects(Bitmap bmp)
    {
      // each of the generated bitmaps will be disposed of by InternalDecode
      yield return new Bitmap(bmp);

      yield return Rotate(bmp, RotateFlipType.Rotate90FlipNone);
      yield return Rotate(bmp, RotateFlipType.Rotate180FlipNone);
      yield return Rotate(bmp, RotateFlipType.Rotate270FlipNone);

      // split the image horizontally in "slices"
      var y = 0;
      while (y < bmp.Height)
      {
        var rect = new Rectangle(0, y, bmp.Width, SLICE_SIZE);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        y += SLICE_SIZE;
      }

      // split the image horizontally in two parts, one growing and the other one shrinking
      y = SLICE_SIZE;
      while (y < bmp.Height)
      {
        var rect = new Rectangle(0, 0, bmp.Width, y);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        rect = new Rectangle(0, y, bmp.Width, bmp.Height - y);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        y += SLICE_SIZE;
      }

      // split the image vertically in "slices"
      var x = 0;
      while (x < bmp.Width)
      {
        var rect = new Rectangle(x, 0, SLICE_SIZE, bmp.Height);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        x += SLICE_SIZE;
      }

      // split the image vertically in two parts, one growing and the other one shrinking
      x = SLICE_SIZE;
      while (x < bmp.Width)
      {
        var rect = new Rectangle(0, 0, x, bmp.Height);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        rect = new Rectangle(x, 0, bmp.Width - x, bmp.Height);
        using (var slice = CropImage(bmp, rect))
          yield return ToGrayscale(slice);

        x += SLICE_SIZE;
      }
    }

    private static Bitmap Rotate(Image bmp, RotateFlipType flipType)
    {
      var bitmap = new Bitmap(bmp);
      bitmap.RotateFlip(flipType);

      return bitmap;
    }

    private static Bitmap CropImage(Image img, Rectangle cropArea)
    {
      var bitmap = new Bitmap(cropArea.Width, cropArea.Height);
      using (var g = Graphics.FromImage(bitmap))
      {
        g.DrawImage(img, -cropArea.X, -cropArea.Y);
        g.Save();

        return bitmap;
      }
    }

    private static Bitmap ToGrayscale(Image img)
    {
      var bitmap = new Bitmap(img.Width, img.Height);
      using (var g = Graphics.FromImage(bitmap))
      {
        var cm = new ColorMatrix(new[]
        {
          new[] {0.3f, 0.3f, 0.3f, 0, 0},
          new[] {0.59f, 0.59f, 0.59f, 0, 0},
          new[] {0.11f, 0.11f, 0.11f, 0, 0},
          new float[] {0, 0, 0, 1, 0, 0},
          new float[] {0, 0, 0, 0, 1, 0},
          new float[] {0, 0, 0, 0, 0, 1}
        });

        var ia = new ImageAttributes();
        ia.SetColorMatrix(cm);
        g.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
        g.Save();

        return bitmap;
      }
    }

    private static string InternalDecode(Bitmap bmp)
    {
      var reader = new BarcodeReader();
      var result = reader.Decode(bmp);
      bmp.Dispose();

      return result == null ? null : result.Text;
    }
  }
}