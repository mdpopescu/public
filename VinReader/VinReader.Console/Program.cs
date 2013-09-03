using System;
using System.Collections;
using System.Drawing;
using Renfield.VinReader.Library;

namespace Renfield.VinReader.Command
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      Console.Write("File name: ");
      var filename = Console.ReadLine();

      using (var image = Image.FromFile(filename))
      using (var bmp = new Bitmap(image))
      {
        var codes = new ArrayList();
        BarcodeImaging.FullScanPageCode39(ref codes, bmp, 50);

        foreach (var code in codes)
          Console.WriteLine(code.ToString());
      }
    }
  }
}