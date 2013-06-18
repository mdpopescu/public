using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Failover.Library;

namespace Renfield.Failover.Tests
{
  [TestClass]
  public class AcceptanceTests
  {
    [TestMethod]
    public void TestFailoverMechanism()
    {
      var stream = new MemoryStream();
      var logger = new StreamLogger(stream);
      logger.SystemClock = () => new DateTime(2000, 1, 1, 14, 15, 16);
      var guard = new FailGuard(logger);

      var success = false;
      guard.Attempt(
        () => { throw new Exception("Call failed."); },
        () => { success = true; });

      Assert.IsTrue(success);
      var lines = stream.GetLines();
      Assert.AreEqual("[2000.01.01 14:15:16 E] Call failed.", lines[0]);
      Assert.AreEqual("[2000.01.01 14:15:16 D] Success.", lines[1]);
    }
  }
}