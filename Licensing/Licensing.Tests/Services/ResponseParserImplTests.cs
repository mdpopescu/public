using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Licensing.Library.Services;

namespace Renfield.Licensing.Tests.Services
{
  [TestClass]
  public class ResponseParserImplTests
  {
    private ResponseParserImpl sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new ResponseParserImpl();
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
  }
}