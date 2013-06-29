using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Services
{
  [TestClass]
  public class DateServiceTests
  {
    private DateService sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new DateService();
    }

    [TestClass]
    public class GetRange : DateServiceTests
    {
      [TestMethod]
      public void Between()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1), new DateTime(2001, 3, 5));

        Assert.AreEqual("records between 2000-01-01 and 2001-03-05", result);
      }

      [TestMethod]
      public void FromDate()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1), null);

        Assert.AreEqual("records after 2000-01-01", result);
      }

      [TestMethod]
      public void ToDate()
      {
        var result = sut.GetRange(null, new DateTime(2001, 3, 5));

        Assert.AreEqual("records before 2001-03-05", result);
      }

      [TestMethod]
      public void AllRecords()
      {
        var result = sut.GetRange(null, null);

        Assert.AreEqual("all records", result);
      }

      [TestMethod]
      public void UseOnlyDates()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1, 2, 3, 4), new DateTime(2001, 3, 5, 6, 7, 8));

        Assert.AreEqual("records between 2000-01-01 and 2001-03-05", result);
      }
    }
  }
}