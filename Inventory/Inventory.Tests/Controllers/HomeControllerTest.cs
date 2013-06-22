using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.Inventory.Controllers;

namespace Renfield.Inventory.Tests.Controllers
{
  [TestClass]
  public class HomeControllerTest
  {
    [TestMethod]
    public void Index()
    {
      // Arrange
      var controller = new HomeController();

      // Act
      var result = controller.Index() as ViewResult;

      // Assert
      Assert.AreEqual("Welcome to Inventory!", result.ViewBag.Message);
    }
  }
}