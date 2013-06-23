using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SafeRedir.Data;

namespace Renfield.SafeRedir.Tests.Data
{
  [TestClass]
  public class UrlInfoTests
  {
    [TestClass]
    public class GetUrl : UrlInfoTests
    {
      [TestMethod]
      public void ReturnsSafeUrlAfterExpirationDate()
      {
        var sut = new UrlInfo { OriginalUrl = "a", SafeUrl = "b", ExpiresAt = new DateTime(2000, 1, 2, 3, 4, 5) };

        var result = sut.GetUrl(new DateTime(2000, 1, 2, 3, 4, 6));

        Assert.AreEqual("b", result);
      }

      [TestMethod]
      public void ReturnsSafeUrlExactlyOnExpirationDate()
      {
        var sut = new UrlInfo { OriginalUrl = "a", SafeUrl = "b", ExpiresAt = new DateTime(2000, 1, 2, 3, 4, 5) };

        var result = sut.GetUrl(new DateTime(2000, 1, 2, 3, 4, 5));

        Assert.AreEqual("b", result);
      }

      [TestMethod]
      public void ReturnsOriginalUrlBeforeExpirationDate()
      {
        var sut = new UrlInfo { OriginalUrl = "a", SafeUrl = "b", ExpiresAt = new DateTime(2000, 1, 2, 3, 4, 5) };

        var result = sut.GetUrl(new DateTime(2000, 1, 2, 3, 4, 4));

        Assert.AreEqual("a", result);
      }
    }
  }
}