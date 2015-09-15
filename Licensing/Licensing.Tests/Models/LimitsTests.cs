using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Models;

namespace Renfield.Licensing.Tests.Models
{
  [TestClass]
  public class LimitsTests
  {
    [TestClass]
    public class GetRemainingDays : LimitsTests
    {
      [TestMethod]
      public void ReturnsMaxValueIfLimitDaysIsMinusOne()
      {
        var sut = new Limits {Days = -1};

        var result = sut.GetRemainingDays(DateTime.Today);

        Assert.AreEqual(int.MaxValue, result);
      }

      [TestMethod]
      public void ReturnsZeroIfExpired()
      {
        var sut = new Limits {Days = 5};

        var result = sut.GetRemainingDays(new DateTime(2000, 1, 1));

        Assert.AreEqual(0, result);
      }

      [TestMethod]
      public void ReturnsNumberOfRemainingDays()
      {
        var sut = new Limits {Days = 5};

        var result = sut.GetRemainingDays(DateTime.Today.AddDays(-2));

        Assert.AreEqual(3, result); // includes today
      }
    }
  }
}