using System;
using System.Drawing;

namespace Renfield.VinReader.Command
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var decoder = new ImageDecoder();

      do
      {
        Console.Write("File name: ");
        var filename = Console.ReadLine();
        if (string.IsNullOrEmpty(filename))
          break;

        using (var bmp = (Bitmap) Image.FromFile(filename))
        {
          var result = decoder.Decode(bmp);
          Console.WriteLine(result ?? "Could not decode " + filename);
        }
      } while (true);
    }
  }
}