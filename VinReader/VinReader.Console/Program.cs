using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        var result = TryDecode(filename);
        Console.WriteLine(result ?? "Could not decode " + filename);
      } while (true);
    }

    private static string TryDecode(string filename)
    {
      using (var bmp = (Bitmap) Image.FromFile(filename))
      {
        //var result = TryRotateAndDecode(bmp);
        //if (result != null)
        //  return result;

        //// split the image horizontally in "slices", 50 pixels high, and try with each slices until one decodes
        //var y = 0;
        //while (y < bmp.Height)
        //{
        //  var rect = new Rectangle(0, y, bmp.Width, SLICE_HEIGHT);
        //  var bitmap = ToGrayscale(CropImage(bmp, rect));
        //  //bitmap.Save(Path.Combine(Path.GetDirectoryName(filename), string.Format("slice-{0}.jpg", y)), ImageFormat.Jpeg);

        //  result = TryRotateAndDecode(bitmap);
        //  if (result != null)
        //    return result;

        //  y += SLICE_HEIGHT;
        //}

        var codes = new ArrayList();
        BarcodeImaging.FullScanPageCode39(ref codes, bmp, 50);
        var candidate = codes
          .Cast<string>()
          .Select(it => new string(it.Where(char.IsLetterOrDigit).ToArray()))
          .Select(it => it.StartsWith("I") ? it.Substring(1) : it)
          .OrderByDescending(it => it.Length)
          .FirstOrDefault();

        return candidate;
      }
    }

    private static string TryRotateAndDecode(Bitmap bmp)
    {
      for (var i = 1; i <= 4; i++)
      {
        var result = Decode(bmp);
        if (result != null)
          return result;

        bmp = Rotate90(bmp);
      }

      return null;
    }

    private static Bitmap Rotate90(Image bmp)
    {
      var bitmap = new Bitmap(bmp.Width, bmp.Height);
      bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

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