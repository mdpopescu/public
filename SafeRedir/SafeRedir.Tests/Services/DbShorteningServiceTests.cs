using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.SafeRedir.Data;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Services
{
  [TestClass]
  public class DbShorteningServiceTests
  {
    private Mock<Repository> repository;
    private Mock<UniqueIdGenerator> idGen;
    private DbShorteningService sut;

    [TestInitialize]
    public void SetUp()
    {
      repository = new Mock<Repository>();
      idGen = new Mock<UniqueIdGenerator>();
      sut = new DbShorteningService(repository.Object, idGen.Object);
    }

    [TestClass]
    public class CreateRedirect : DbShorteningServiceTests
    {
      [TestMethod]
      public void SavesUrlInfo()
      {
        SystemInfo.SystemClock = () => new DateTime(2000, 1, 1, 1, 1, 1);

        sut.CreateRedirect("example.com", "safe.com", 10);

        repository.Verify(it => it.AddUrlInfo(It.Is<UrlInfo>(ui => ui.OriginalUrl == "example.com" &&
                                                                   ui.SafeUrl == "safe.com" &&
                                                                   ui.ExpiresAt == new DateTime(2000, 1, 1, 1, 1, 11))));
        repository.Verify(it => it.SaveChanges());
      }

      [TestMethod]
      public void SavesAndReturnsIdFromUniqueIdService()
      {
        idGen
          .Setup(it => it.Generate())
          .Returns("123");

        var result = sut.CreateRedirect("example.com", "safe.com", 10);

        Assert.AreEqual("123", result);
        repository.Verify(it => it.AddUrlInfo(It.Is<UrlInfo>(ui => ui.Id == "123")));
      }

      [TestMethod]
      public void RetriesIfUniqueIdAlreadyExists()
      {
        var ids = new List<string> { "123", "456" };
        repository
          .Setup(it => it.GetUrlInfo("123"))
          .Returns(new UrlInfo());
        var index = 0;
        idGen
          .Setup(it => it.Generate())
          .Returns(() => ids[index++]);

        var result = sut.CreateRedirect("example.com", "safe.com", 10);

        Assert.AreEqual("456", result);
      }
    }

    [TestClass]
    public class GetUrl : DbShorteningServiceTests
    {
      [TestMethod]
      public void ReturnsOriginalUrlAsTemporaryRedirect()
      {
        repository
          .Setup(it => it.GetUrlInfo("123"))
          .Returns(new UrlInfo { OriginalUrl = "a", SafeUrl = "b", ExpiresAt = new DateTime(2000, 1, 1, 1, 1, 1) });
        SystemInfo.SystemClock = () => new DateTime(2000, 1, 1, 1, 1, 0);

        var result = sut.GetUrl("123");

        Assert.AreEqual("a", result.Url);
        Assert.IsFalse(result.Permanent);
      }

      [TestMethod]
      public void ReturnsSafeUrlAsPermanentRedirect()
      {
        repository
          .Setup(it => it.GetUrlInfo("123"))
          .Returns(new UrlInfo { OriginalUrl = "a", SafeUrl = "b", ExpiresAt = new DateTime(2000, 1, 1, 1, 1, 1) });
        SystemInfo.SystemClock = () => new DateTime(2000, 1, 1, 1, 1, 2);

        var result = sut.GetUrl("123");

        Assert.AreEqual("b", result.Url);
        Assert.IsTrue(result.Permanent);
      }

      [TestMethod]
      public void ReturnsNullForUnknownId()
      {
        var result = sut.GetUrl("123");

        Assert.IsNull(result);
      }
    }
  }
}