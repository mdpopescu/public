using System;
using System.IO;
using System.Text;
using System.Threading;
using BigDataProcessing.Library.Models;
using BigDataProcessing.Library.Services;
using BigDataProcessing.Tests.Helper;
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
        var loader = new RxTextStreamReader();
        var writer = new RxTextStreamWriter();

        var processors = new[]
        {
          new HelperConverter(),
        };

        var app = new App(new NullLogger(), loader, writer, processors);

        var config = new Configuration
        {
          Input = input,
          Output = output,
          Threads = 1,
        };

        app.Run(config);

        output.Seek(0, SeekOrigin.Begin);
        var result = Encoding.UTF8.GetString(output.ToArray());

        Assert.AreEqual("2:4:6\r\n8:10:12\r\n", result);
      }
    }

    [TestMethod]
    public void MultiThreadedTest()
    {
      using (var input = new MemoryStream(Encoding.UTF8.GetBytes(Resources.LargerFile)))
      using (var output = new MemoryStream())
      {
        var loader = new RxTextStreamReader();
        var writer = new RxTextStreamWriter();

        var processors = new[]
        {
          new NullConverter(),
        };

        var app = new App(new NullLogger(), loader, writer, processors);

        var config = new Configuration
        {
          Input = input,
          Output = output,
          Threads = 3,
        };

        app.Run(config);

        output.Seek(0, SeekOrigin.Begin);
        var result = Encoding.UTF8.GetString(output.ToArray());
        var actualLines = result.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        var expectedLines = Resources.LargerFile.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

        CollectionAssert.AreEqual(expectedLines, actualLines);
      }
    }
  }
}