using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Inventory.Controllers;
using Renfield.Inventory.Data;
using Renfield.Inventory.Models;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Tests.Controllers
{
  [TestClass]
  public class AcquisitionsControllerTests
  {
    private Mock<Logic> logic;
    private AcquisitionsController sut;

    [TestInitialize]
    public void SetUp()
    {
      logic = new Mock<Logic>();
      sut = new AcquisitionsController(logic.Object);
    }

    [TestClass]
    public class Index : AcquisitionsControllerTests
    {
      [TestMethod]
      public void ReturnsViewWithNoModel()
      {
        var result = sut.Index() as ViewResult;

        Assert.IsNull(result.Model);
      }
    }

    [TestClass]
    public class GetStock : AcquisitionsControllerTests
    {
      [TestMethod]
      public void ReturnsDataFromServiceAsJson()
      {
        logic
          .Setup(it => it.GetAcquisitions())
          .Returns(new[]
          {
            new Acquisition { Company = new Company { Name = "Microsoft" }, Date = new DateTime(2000, 3, 4) },
            new Acquisition { Company = new Company { Name = "Borland" }, Date = new DateTime(2000, 5, 6) },
          });

        var result = sut.GetAcquisitions() as JsonResult;

        var data = ((IEnumerable<AcquisitionModel>) result.Data).ToList();
        Assert.AreEqual(2, data.Count);
        Assert.AreEqual("Microsoft", data[0].CompanyName);
        Assert.AreEqual(new DateTime(2000, 3, 4), data[0].Date);
      }
    }
  }
}