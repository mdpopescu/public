using System;
using System.Drawing;
using ZXing;

namespace Renfield.VinReader.Command
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      do
      {
        Console.Write("File name: ");
        var filename = Console.ReadLine();
        if (string.IsNullOrEmpty(filename))
          break;

        using (var bmp = (Bitmap) Image.FromFile(filename))
        {
          var reader = new BarcodeReader();
          var result = reader.Decode(bmp);
          if (result == null)
            Console.WriteLine("Could not decode " + filename);
          else
            Console.WriteLine("{0} => {1}", result.BarcodeFormat, result.Text);
        }
      } while (true);
    }
  }
}