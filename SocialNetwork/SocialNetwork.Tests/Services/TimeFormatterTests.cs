using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork.Library.Services;

namespace SocialNetwork.Tests.Services
{
  [TestClass]
  public class TimeFormatterTests
  {
    private TimeFormatter sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new TimeFormatter();
    }

    [TestMethod]
    public void ReturnsOneSecond()
    {
      var result = sut.Format(new TimeSpan(0, 0, 1));

      Assert.AreEqual("1 second ago", result);
    }

    [TestMethod]
    public void Returns23Seconds()
    {
      var result = sut.Format(new TimeSpan(0, 0, 23));

      Assert.AreEqual("23 seconds ago", result);
    }

    [TestMethod]
    public void ReturnsOneMinute()
    {
      var result = sut.Format(new TimeSpan(0, 1, 0));

      Assert.AreEqual("1 minute ago", result);
    }

    [TestMethod]
    public void ReturnsFiveMinutes()
    {
      var result = sut.Format(new TimeSpan(0, 5, 0));

      Assert.AreEqual("5 minutes ago", result);
    }

    [TestMethod]
    public void IgnoresSecondsWhenReturningMinutes()
    {
      var result = sut.Format(new TimeSpan(0, 12, 33));

      Assert.AreEqual("12 minutes ago", result);
    }
  }
}