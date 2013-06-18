using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Failover.Library;

namespace Renfield.Failover.Tests
{
  [TestClass]
  public class StreamLoggerTests
  {
    [TestInitialize]
    public void SetUp()
    {
      stream = new MemoryStream();
      sut = new StreamLogger(stream) { SystemClock = () => new DateTime(2000, 1, 2, 3, 4, 5) };
    }

    [TestClass]
    public class Debug : StreamLoggerTests
    {
      [TestMethod]
      public void WritesTheGivenMessage()
      {
        const string MESSAGE = "abc123";

        sut.Debug(MESSAGE);

        var lines = stream.GetLines();
        Assert.IsTrue(lines[0].EndsWith(MESSAGE));
      }

      [TestMethod]
      public void WritesTheCurrentDateTime()
      {
        sut.Debug("");

        var lines = stream.GetLines();
        Assert.IsTrue(lines[0].StartsWith("[2000.01.02 03:04:05"));
      }

      [TestMethod]
      public void Writes_D_AtTheEndOfThePrefix()
      {
        sut.Debug("");

        var lines = stream.GetLines();
        Assert.AreEqual("D]", lines[0].Substring(21, 2));
      }
    }

    [TestClass]
    public class Error: StreamLoggerTests
    {
      [TestMethod]
      public void WritesTheGivenMessage()
      {
        const string MESSAGE = "abc123";

        sut.Error(new Exception(MESSAGE));

        var lines = stream.GetLines();
        Assert.IsTrue(lines[0].EndsWith(MESSAGE));
      }

      [TestMethod]
      public void WritesTheCurrentDateTime()
      {
        sut.Error(new Exception(""));

        var lines = stream.GetLines();
        Assert.IsTrue(lines[0].StartsWith("[2000.01.02 03:04:05"));
      }

      [TestMethod]
      public void Writes_E_AtTheEndOfThePrefix()
      {
        sut.Error(new Exception(""));

        var lines = stream.GetLines();
        Assert.AreEqual("E]", lines[0].Substring(21, 2));
      }
    }

    //

    private Stream stream;
    private StreamLogger sut;
  }
}