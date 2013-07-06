using System.IO;
using Gma.QrCodeNet.Encoding;

namespace Renfield.QRMaker
{
  internal class Program
  {
    private static void Main(string[] args)
    {
      const string content = "http://mdpopescu.blogspot.com";

      var generator = new Generator(new QrEncoder());

      using (var ms = new MemoryStream())
      {
        generator.Render(content, ms);
        ms.Seek(0, SeekOrigin.Begin);

        using (var file = new FileStream("test.png", FileMode.Create, FileAccess.Write, FileShare.None))
        {
          ms.CopyTo(file);
          file.Flush();
        }
      }
    }
  }
}