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

        Assert.AreEqual("records between 01/01/2000 and 03/05/2001", result);
      }

      [TestMethod]
      public void FromDate()
      {
        var result = sut.GetRange(new DateTime(2000, 1, 1), null);

        Assert.AreEqual("records after 01/01/2000", result);
      }

      [TestMethod]
      public void ToDate()
      {
        var result = sut.GetRange(null, new DateTime(2001, 3, 5));

        Assert.AreEqual("records before 03/05/2001", result);
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

        Assert.AreEqual("records between 01/01/2000 and 03/05/2001", result);
      }
    }
  }
}