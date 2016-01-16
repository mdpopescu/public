using System.IO;
using System.Reactive.Linq;
using System.Text;
using BigDataProcessing.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataProcessing.Tests.Services
{
  [TestClass]
  public class RxTextStreamWriterTests
  {
    private RxTextStreamWriter sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new RxTextStreamWriter();
    }

    [TestMethod]
    public void WritesALineToTheStream()
    {
      using (var ms = new MemoryStream())
      {
        var data = new[] { "line1" };

        sut.Write(ms, data.ToObservable());

        ms.Seek(0, SeekOrigin.Begin);
        var result = Encoding.UTF8.GetString(ms.ToArray());
        Assert.AreEqual("line1\r\n", result);
      }
    }

    [TestMethod]
    public void WritesMultipleLinesToTheStream()
    {
      using (var ms = new MemoryStream())
      {
        var data = new[] { "line1", "line2" };

        sut.Write(ms, data.ToObservable());

        ms.Seek(0, SeekOrigin.Begin);
        var result = Encoding.UTF8.GetString(ms.ToArray());
        Assert.AreEqual("line1\r\nline2\r\n", result);
      }
    }

    [TestMethod]
    public void DoesNotCloseTheStream()
    {
      using (var ms = new MemoryStream())
      {
        var data = new[] { "line1" };

        sut.Write(ms, data.ToObservable());

        // This does not throw an exception
        ms.Seek(0, SeekOrigin.Begin);
      }
    }
  }
}