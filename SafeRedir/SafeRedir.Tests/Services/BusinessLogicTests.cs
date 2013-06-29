using System;
using System.Collections.Generic;
using System.Linq;
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
      sut = new BusinessLogic(repository.Object, idGen.Object, new DateService());
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

    [TestClass]
    public class GetRecords : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsRecordsMatchingDateRange()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { ExpiresAt = new DateTime(1999, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2000, 3, 1) },
            new UrlInfo { ExpiresAt = new DateTime(2001, 1, 1) },
          });

        var result = sut.GetRecords(null, new DateTime(2000, 1, 1), new DateTime(2000, 12, 31));

        Assert.AreEqual("records between 2000-01-01 and 2000-12-31", result.DateRange);
        var records = result.UrlInformation.ToList();
        Assert.AreEqual(3, records.Count);
        Assert.AreEqual(2000, records[0].ExpiresAt.Year);
        Assert.AreEqual(2000, records[1].ExpiresAt.Year);
        Assert.AreEqual(2000, records[2].ExpiresAt.Year);
      }

      [TestMethod]
      public void ReturnsRecordsInDescendingOrderByDate()
      {
        repository
          .Setup(it => it.GetAll())
          .Returns(new[]
          {
            new UrlInfo { OriginalUrl = "a", ExpiresAt = new DateTime(2000, 1, 1) },
            new UrlInfo { OriginalUrl = "b", ExpiresAt = new DateTime(2000, 2, 1) },
            new UrlInfo { OriginalUrl = "c", ExpiresAt = new DateTime(2000, 3, 1) },
          });

        var result = sut.GetRecords(null, null, null);

        var records = result.UrlInformation.ToList();
        Assert.AreEqual("c", records[0].OriginalUrl);
        Assert.AreEqual("b", records[1].OriginalUrl);
        Assert.AreEqual("a", records[2].OriginalUrl);
      }

      [TestMethod]
      public void ReturnsFirstPageWith10Records()
      {
        var index = 0;
        var list = Enumerable
          .Range(0, 20)
          .Select(_ => new UrlInfo { OriginalUrl = index.ToString(), ExpiresAt = new DateTime(2000, 1, 1).AddDays(index++) })
          .ToList();
        repository
          .Setup(it => it.GetAll())
          .Returns(list);

        var result = sut.GetRecords(null, null, null);

        Assert.AreEqual(2, result.PageCount);
        Assert.AreEqual(1, result.CurrentPage);
        var records = result.UrlInformation.ToList();
        Assert.AreEqual(10, records.Count);
        Assert.AreEqual(20, records[0].ExpiresAt.Day);
        Assert.AreEqual(11, records[9].ExpiresAt.Day);
      }

      [TestMethod]
      public void ReturnsGivenPage()
      {
        var index = 0;
        var list = Enumerable
          .Range(0, 30)
          .Select(_ => new UrlInfo { OriginalUrl = index.ToString(), ExpiresAt = new DateTime(2000, 1, 1).AddDays(index++) })
          .ToList();
        repository
          .Setup(it => it.GetAll())
          .Returns(list);

        var result = sut.GetRecords(2, null, null);

        Assert.AreEqual(2, result.CurrentPage);
        var records = result.UrlInformation.ToList();
        Assert.AreEqual(20, records[0].ExpiresAt.Day);
        Assert.AreEqual(11, records[9].ExpiresAt.Day);
      }
    }
  }
}