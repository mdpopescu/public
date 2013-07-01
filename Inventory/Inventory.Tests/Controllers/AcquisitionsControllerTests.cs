using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Inventory.Controllers;
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

    [TestMethod]
    public void IndexReturnsViewWithNoModel()
    {
      var result = sut.Index() as ViewResult;

      Assert.IsNull(result.Model);
    }

    [TestMethod]
    public void GetAcquisitionsReturnsDataFromServiceAsJson()
    {
      var list = new List<AcquisitionModel>();
      logic
        .Setup(it => it.GetAcquisitions())
        .Returns(list);

      var result = sut.GetAcquisitions() as JsonResult;

      Assert.AreEqual(list, result.Data);
    }

    [TestMethod]
    public void GetAcquisitionItemsReturnsDataFromServiceAsJson()
    {
      var list = new List<AcquisitionItemModel>();
      logic
        .Setup(it => it.GetAcquisitionItems(1))
        .Returns(list);

      var result = sut.GetAcquisitionItems(1) as JsonResult;

      Assert.AreEqual(list, result.Data);
    }

    [TestMethod]
    public void GetCreateReturnsViewWithAcquisitionModel()
    {
      var result = sut.Create() as ViewResult;

      var model = result.Model as AcquisitionModel;
      Assert.IsNotNull(model);
      Assert.AreEqual(DateTime.Today.ToString(Constants.DATE_FORMAT), model.Date);
    }

    [TestMethod]
    public void PostCreateRedirectsBackToGet()
    {
      var result = sut.Create(new AcquisitionModel()) as RedirectToRouteResult;

      Assert.AreEqual("Create", result.RouteValues["action"]);
      Assert.IsNull(result.RouteValues["controller"]);
    }
  }
}