using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.SafeRedir.Data;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Services
{
  [TestClass]
  public class BusinessLogicTests
  {
    private Mock<Repository> repository;
    private Mock<UniqueIdGenerator> idGen;
    private BusinessLogic sut;

    [TestInitialize]
    public void SetUp()
    {
      repository = new Mock<Repository>();
      idGen = new Mock<UniqueIdGenerator>();
      sut = new BusinessLogic(repository.Object, idGen.Object);
    }

    [TestClass]
    public class CreateRedirect : BusinessLogicTests
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
    public class GetUrl : BusinessLogicTests
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

    [TestClass]
    public class GetSummary : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsCountForCurrentDay()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 2) },
          });
        SystemInfo.SystemClock = () => new DateTime(2000, 1, 1);

        var result = sut.GetSummary();

        Assert.AreEqual(3, result.Today);
      }

      [TestMethod]
      public void ReturnsCountForCurrentMonth()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 3, 2) },
          });
        SystemInfo.SystemClock = () => new DateTime(2000, 2, 1);

        var result = sut.GetSummary();

        Assert.AreEqual(2, result.CurrentMonth);
      }

      [TestMethod]
      public void ReturnsCountForCurrentYear()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 3, 2) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 3, 5) },
          });
        SystemInfo.SystemClock = () => new DateTime(2001, 2, 1);

        var result = sut.GetSummary();

        Assert.AreEqual(3, result.CurrentYear);
      }

      [TestMethod]
      public void ReturnsOverallCount()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 3, 2) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 3, 5) },
          });
        SystemInfo.SystemClock = () => new DateTime(2001, 2, 1);

        var result = sut.GetSummary();

        Assert.AreEqual(5, result.Overall);
      }
    }
  }
}