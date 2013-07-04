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
  public class SalesControllerTests
  {
    private Mock<Logic> logic;
    private SalesController sut;

    [TestInitialize]
    public void SetUp()
    {
      logic = new Mock<Logic>();
      sut = new SalesController(logic.Object);
    }

    [TestMethod]
    public void IndexReturnsViewWithNoModel()
    {
      var result = sut.Index() as ViewResult;

      result.Model.Should().BeNull();
    }

    [TestMethod]
    public void GetSalesReturnsDataFromServiceAsJson()
    {
      var list = new List<SaleModel>();
      logic
        .Setup(it => it.GetSales())
        .Returns(list);

      var result = sut.GetSales() as JsonResult;

      result.Data.As<IEnumerable<SaleModel>>().ShouldAllBeEquivalentTo(list);
    }

    [TestMethod]
    public void GetSaleItemsReturnsDataFromServiceAsJson()
    {
      var list = new List<SaleItemModel>();
      logic
        .Setup(it => it.GetSaleItems(1))
        .Returns(list);

      var result = sut.GetSaleItems(1) as JsonResult;

      result.Data.As<IEnumerable<SaleItemModel>>().ShouldAllBeEquivalentTo(list);
    }

    [TestMethod]
    public void GetCreateReturnsViewWithSaleModel()
    {
      var result = sut.Create() as ViewResult;

      var model = result.Model as SaleModel;
      model.Should().NotBeNull();
      model.Date.Should().Be(DateTime.Today.ToString(Constants.DATE_FORMAT));
    }

    [TestMethod]
    public void PostCreateRedirectsBackToGet()
    {
      var result = sut.Create(new SaleModel()) as RedirectToRouteResult;

      result.RouteValues["action"].Should().Be("Create");
      result.RouteValues["controller"].Should().BeNull();
    }

    [TestMethod]
    public void PostCallsTheLogicToAddTheNewSale()
    {
      var model = new SaleModel();

      sut.Create(model);

      logic.Verify(it => it.AddSale(model));
    }
  }
}