using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentAssertions;
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

      result.Model.Should().BeNull();
    }

    [TestMethod]
    public void GetAcquisitionsReturnsDataFromServiceAsJson()
    {
      var list = new List<AcquisitionModel>();
      logic
        .Setup(it => it.GetAcquisitions())
        .Returns(list);

      var result = sut.GetAcquisitions() as JsonResult;

      result.Data.As<IEnumerable<AcquisitionModel>>().ShouldAllBeEquivalentTo(list);
    }

    [TestMethod]
    public void GetAcquisitionItemsReturnsDataFromServiceAsJson()
    {
      var list = new List<AcquisitionItemModel>();
      logic
        .Setup(it => it.GetAcquisitionItems(1))
        .Returns(list);

      var result = sut.GetAcquisitionItems(1) as JsonResult;

      result.Data.As<IEnumerable<AcquisitionItemModel>>().ShouldAllBeEquivalentTo(list);
    }

    [TestMethod]
    public void GetCreateReturnsViewWithAcquisitionModel()
    {
      var result = sut.Create() as ViewResult;

      var model = result.Model as AcquisitionModel;
      model.Should().NotBeNull();
      model.Date.Should().Be(DateTime.Today.ToString(Constants.DATE_FORMAT));
    }

    [TestMethod]
    public void PostCreateRedirectsBackToGet()
    {
      var result = sut.Create(new AcquisitionModel()) as RedirectToRouteResult;

      result.RouteValues["action"].Should().Be("Create");
      result.RouteValues["controller"].Should().BeNull();
    }

    [TestMethod]
    public void PostCallsTheLogicToAddTheNewAcquisition()
    {
      var model = new AcquisitionModel();

      sut.Create(model);

      logic.Verify(it => it.AddAcquisition(model));
    }
  }
}