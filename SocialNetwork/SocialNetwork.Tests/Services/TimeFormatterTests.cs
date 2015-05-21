using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetwork.Library.Services;

namespace SocialNetwork.Tests.Services
{
  [TestClass]
  public class TimeFormatterTests
  {
    [TestMethod]
    public void ReturnsOneSecond()
    {
      var result = TimeFormatter.Format(new TimeSpan(0, 0, 1));

      Assert.AreEqual("1 second ago", result);
    }

    [TestMethod]
    public void Returns23Seconds()
    {
      var result = TimeFormatter.Format(new TimeSpan(0, 0, 23));

      Assert.AreEqual("23 seconds ago", result);
    }

    [TestMethod]
    public void ReturnsOneMinute()
    {
      var result = TimeFormatter.Format(new TimeSpan(0, 1, 0));

      Assert.AreEqual("1 minute ago", result);
    }

    [TestMethod]
    public void ReturnsFiveMinutes()
    {
      var result = TimeFormatter.Format(new TimeSpan(0, 5, 0));

      Assert.AreEqual("5 minutes ago", result);
    }

    [TestMethod]
    public void IgnoresSecondsWhenReturningMinutes()
    {
      var result = TimeFormatter.Format(new TimeSpan(0, 12, 33));

      Assert.AreEqual("12 minutes ago", result);
    }

    [TestMethod]
    public void ReturnsOneHour()
    {
      var result = TimeFormatter.Format(new TimeSpan(1, 0, 0));

      Assert.AreEqual("1 hour ago", result);
    }

    [TestMethod]
    public void ReturnsTwoHours()
    {
      var result = TimeFormatter.Format(new TimeSpan(2, 0, 0));

      Assert.AreEqual("2 hours ago", result);
    }

    [TestMethod]
    public void IgnoresMinutesAndSecondsWhenReturningHours()
    {
      var result = TimeFormatter.Format(new TimeSpan(3, 4, 5));

      Assert.AreEqual("3 hours ago", result);
    }
  }
}