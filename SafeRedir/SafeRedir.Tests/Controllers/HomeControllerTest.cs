using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SafeRedir.Controllers;

namespace Renfield.SafeRedir.Tests.Controllers
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
      Assert.AreEqual("Safe (time-limited) URL redirector", result.ViewBag.Message);
    }
  }
}