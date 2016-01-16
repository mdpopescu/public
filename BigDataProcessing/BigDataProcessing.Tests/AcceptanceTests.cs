using System.IO;
using System.Text;
using BigDataProcessing.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataProcessing.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void SmokeTest()
    {
      using (var input = new MemoryStream(Encoding.UTF8.GetBytes(Resources.SmallFile)))
      using (var output = new MemoryStream())
      {
        var loader = new RxStreamReader(input);

        var processors = new[]
        {
          new CsvReader(),
          new HelperConverter(),
        };

        var writer = new RxStreamWriter(output);

        var app = new App(loader, processors, writer);

        var config = new Configuration
        {
          Threads = 1,
        };

        app.Run(config);

        output.Seek(0, SeekOrigin.Begin);
        var result = Encoding.UTF8.GetString(output.ToArray());

        Assert.AreEqual("2:4:6\r\n8:10:12\r\n", result);
      }
    }
  }
}