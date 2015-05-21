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
  }
}