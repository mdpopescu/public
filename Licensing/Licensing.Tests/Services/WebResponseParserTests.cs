using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class WebResponseParserTests
  {
    private WebResponseParser sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new WebResponseParser();
    }

    [TestMethod]
    public void ReturnsKeyForCorrectlyFormattedResponse()
    {
      var result = sut.Parse("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

      Assert.AreEqual("{D98F6376-94F7-4D82-AA37-FC00F0166700}", result.Key);
    }

    [TestMethod]
    public void ReturnsDateForCorrectlyFormattedResponse()
    {
      var result = sut.Parse("{D98F6376-94F7-4D82-AA37-FC00F0166700} 9999-12-31");

      Assert.AreEqual(new DateTime(9999, 12, 31), result.Expiration);
    }

    [TestMethod]
    public void ReturnsEmptyValuesForIncorrectlyFormattedResponse()
    {
      var result = sut.Parse("abc");

      Assert.IsNotNull(result);
      Assert.IsTrue(string.IsNullOrEmpty(result.Key));
      Assert.AreEqual(DateTime.MinValue, result.Expiration);
    }

    [TestMethod]
    public void IgnoresSurroundingQuotes()
    {
      var result = sut.Parse("\"192ff11b-91d0-487d-b297-22d8f5c0ec30 2016-09-19\"");

      Assert.AreEqual("192ff11b-91d0-487d-b297-22d8f5c0ec30", result.Key);
      Assert.AreEqual(new DateTime(2016, 9, 19), result.Expiration);
    }
  }
}