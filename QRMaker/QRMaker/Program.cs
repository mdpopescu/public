using Gma.QrCodeNet.Encoding;
using Ionic.Zip;

namespace Renfield.QRMaker
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      var generator = new Generator(new QrEncoder());
      for (var i = 1; i <= 3; i++)
        generator.Render("Some data " + i, i + ".png");

      var zipper = new ZipFile("123.zip");
      for (var i = 1; i <= 3; i++)
        zipper.AddFile(i + ".png");
      zipper.Save();
    }
  }
}