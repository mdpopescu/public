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
  public class StocksControllerTests
  {
    private Mock<Logic> logic;
    private StocksController sut;

    [TestInitialize]
    public void SetUp()
    {
      logic = new Mock<Logic>();
      sut = new StocksController(logic.Object);
    }

    [TestClass]
    public class Index : StocksControllerTests
    {
      [TestMethod]
      public void ReturnsViewWithNoModel()
      {
        var result = sut.Index() as ViewResult;

        Assert.IsNull(result.Model);
      }
    }

    [TestClass]
    public class GetStocks : StocksControllerTests
    {
      [TestMethod]
      public void ReturnsDataFromServiceAsJson()
      {
        logic
          .Setup(it => it.GetStocks())
          .Returns(new[]
          {
            new Stock { Name = "Hammer", Quantity = 1.00m, PurchaseValue = 5.99m, SaleValue = 7.99m },
            new Stock { Name = "Nails Pack x100", Quantity = 2.00m, PurchaseValue = 0.02m, SaleValue = 0.05m }
          });

        var result = sut.GetStocks() as JsonResult;

        var data = ((IEnumerable<StockModel>) result.Data).ToList();
        Assert.AreEqual(2, data.Count);
        Assert.AreEqual("Hammer", data[0].Name);
        Assert.AreEqual("1.00", data[0].Quantity);
        Assert.AreEqual("5.99", data[0].PurchaseValue);
        Assert.AreEqual("7.99", data[0].SaleValue);
      }
    }
  }
}