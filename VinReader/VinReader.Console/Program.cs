using System;
using System.Drawing;
using System.Drawing.Imaging;
using ZXing;

namespace Renfield.VinReader.Command
{
  internal class Program
  {
    private const int SLICE_HEIGHT = 50;

    private static void Main(string[] args)
    {
      do
      {
        Console.Write("File name: ");
        var filename = Console.ReadLine();
        if (string.IsNullOrEmpty(filename))
          break;

        TryDecode(filename, Console.WriteLine, name => Console.WriteLine("Could not decode " + name));
      } while (true);
    }

    private static void TryDecode(string filename, Action<string> onSuccess, Action<string> onFailure)
    {
      using (var bmp = (Bitmap) Image.FromFile(filename))
      {
        var result = TryInvertFlipAndDecode(bmp);
        if (result != null)
        {
          onSuccess(result);
          return;
        }

        // split the image horizontally in "slices", 50 pixels high, and try with each slices until one decodes
        var y = 0;
        while (y < bmp.Height)
        {
          var rect = new Rectangle(0, y, bmp.Width, SLICE_HEIGHT);
          var bitmap = ToGrayscale(CropImage(bmp, rect));
          //bitmap.Save(Path.Combine(Path.GetDirectoryName(filename), string.Format("slice-{0}.jpg", y)), ImageFormat.Jpeg);

          result = TryInvertFlipAndDecode(bitmap);
          if (result != null)
          {
            onSuccess(result);
            return;
          }

          y += SLICE_HEIGHT;
        }

        onFailure(filename);
      }
    }

    private static string TryInvertFlipAndDecode(Bitmap bmp)
    {
      var result = Decode(bmp) ?? Decode(Invert(bmp)) ?? Decode(Flip(bmp)) ?? Decode(Flip(Invert(bmp)));

      return result;
    }

    private static Bitmap Invert(Image bmp)
    {
      var ia = new ImageAttributes();
      var cm = new ColorMatrix();
      cm.Matrix00 = cm.Matrix11 = cm.Matrix22 = 0.99f;
      cm.Matrix33 = cm.Matrix44 = 1;
      cm.Matrix40 = cm.Matrix41 = cm.Matrix42 = .04f;
      ia.SetColorMatrix(cm);

      var bitmap = new Bitmap(bmp.Width, bmp.Height);
      using (var g = Graphics.FromImage(bitmap))
      {
        g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);

        return bitmap;
      }
    }

    private static Bitmap Flip(Image bmp)
    {
      var bitmap = new Bitmap(bmp.Width, bmp.Height);
      bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);

      return bitmap;
    }

    private static Bitmap CropImage(Image img, Rectangle cropArea)
    {
      var bitmap = new Bitmap(cropArea.Width, cropArea.Height);
      using (var g = Graphics.FromImage(bitmap))
      {
        g.DrawImage(img, -cropArea.X, -cropArea.Y);

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

        return bitmap;
      }
    }

    private static string Decode(Bitmap bmp)
    {
      var reader = new BarcodeReader();
      var result = reader.Decode(bmp);

      return result == null ? null : result.Text;
    }
  }
}