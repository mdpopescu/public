using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BigDataProcessing.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BigDataProcessing.Tests.Services
{
  [TestClass]
  public class RxTextStreamReaderTests
  {
    private TextStreamReader sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new TextStreamReader();
    }

    [TestMethod]
    public void ReturnsTheFirstLine()
    {
      var input = new MemoryStream(Encoding.UTF8.GetBytes("line1"));

      var result = sut.Read(input).ToList();

      Assert.AreEqual(1, result.Count);
      Assert.AreEqual("line1", result[0]);
    }

    [TestMethod]
    public void ReturnsMultipleLines()
    {
      var input = new MemoryStream(Encoding.UTF8.GetBytes("line1\r\nline2\r\nline3\r\n"));

      var result = sut.Read(input).ToList();

      Assert.AreEqual(3, result.Count);
      Assert.AreEqual("line1", result[0]);
      Assert.AreEqual("line2", result[1]);
      Assert.AreEqual("line3", result[2]);
    }

    [TestMethod]
    public void DoesNotCloseTheStream()
    {
      using (var input = new MemoryStream(Encoding.UTF8.GetBytes("")))
      {
        sut.Read(input).ToList();

        // This does not throw an exception
        input.Seek(0, SeekOrigin.Begin);
      }
    }

    [TestMethod]
    public void RethrowsAnExceptionIfTheUnderlyingStreamThrowsOnRead()
    {
      var input = new Mock<Stream>();
      input
        .Setup(it => it.CanRead)
        .Returns(true);
      input
        .Setup(it => it.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
        .Throws(new Exception("error"));

      try
      {
        sut.Read(input.Object).ToList();
        Assert.Fail("Expected an exception to be thrown but it wasn't.");
      }
      catch (Exception ex)
      {
        Assert.AreEqual("error", ex.Message);
      }
    }
  }
}