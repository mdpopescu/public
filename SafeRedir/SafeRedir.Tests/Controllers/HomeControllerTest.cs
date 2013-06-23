using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Renfield.SafeRedir.Controllers;
using Renfield.SafeRedir.Models;

namespace Renfield.SafeRedir.Tests.Controllers
{
  [TestClass]
  public class HomeControllerTest
  {
    [TestClass]
    public class Index : HomeControllerTest
    {
      [TestMethod]
      public void DefaultValues()
      {
        var sut = new HomeController();

        var result = sut.Index() as ViewResult;

        var model = result.Model as RedirectInfo;
        Assert.IsNotNull(model);
        Assert.AreEqual("", model.URL);
        Assert.AreEqual("http://www.randomkittengenerator.com/", model.SafeURL);
        Assert.AreEqual(5 * 60, model.TTL);
      }
    }
  }
}