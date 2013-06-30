using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Inventory.Controllers;

namespace Renfield.Inventory.Tests.Controllers
{
  [TestClass]
  public class StockControllerTests
  {
    private StockController sut;

    [TestInitialize]
    public void SetUp()
    {
      sut = new StockController();
    }

    [TestClass]
    public class Index : StockControllerTests
    {
      [TestMethod]
      public void ReturnsViewWithNoModel()
      {
        var result = sut.Index() as ViewResult;

        Assert.IsNull(result.Model);
      }
    }
  }
}