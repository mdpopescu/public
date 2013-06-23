using System;
using System.Data.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.SafeRedir.Data;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Services
{
  [TestClass]
  public class DbShorteningServiceTests
  {
    [TestClass]
    public class CreateRedirect : DbShorteningServiceTests
    {
      [TestMethod]
      public void SavesUrlInfo()
      {
        var urlInfo = new Mock<IDbSet<UrlInfo>>();
        var repository = new Mock<Repository>();
        repository
          .SetupGet(it => it.UrlInformation)
          .Returns(urlInfo.Object);
        var idGen = new Mock<UniqueIdGenerator>();
        var sut = new DbShorteningService(repository.Object, idGen.Object);
        SystemInfo.SystemClock = () => new DateTime(2000, 1, 1, 1, 1, 1);

        sut.CreateRedirect("example.com", "safe.com", 10);

        urlInfo.Verify(it => it.Add(It.Is<UrlInfo>(ui => ui.OriginalUrl == "example.com" &&
                                                         ui.SafeUrl == "safe.com" &&
                                                         ui.ExpiresAt == new DateTime(2000, 1, 1, 1, 1, 11))));
        repository.Verify(it => it.SaveChanges());
      }

      [TestMethod]
      public void SavesAndReturnsIdFromUniqueIdService()
      {
        var urlInfo = new Mock<IDbSet<UrlInfo>>();
        var repository = new Mock<Repository>();
        repository
          .SetupGet(it => it.UrlInformation)
          .Returns(urlInfo.Object);
        var idGen = new Mock<UniqueIdGenerator>();
        idGen
          .Setup(it => it.Generate())
          .Returns("123");
        var sut = new DbShorteningService(repository.Object, idGen.Object);

        var result = sut.CreateRedirect("example.com", "safe.com", 10);

        Assert.AreEqual("123", result);
        urlInfo.Verify(it => it.Add(It.Is<UrlInfo>(ui => ui.Id == "123")));
      }
    }
  }
}