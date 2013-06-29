using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.SafeRedir.Controllers;
using Renfield.SafeRedir.Models;
using Renfield.SafeRedir.Services;

namespace Renfield.SafeRedir.Tests.Controllers
{
  [TestClass]
  public class HomeControllerTests
  {
    private Mock<Logic> svc;
    private HomeController sut;

    [TestInitialize]
    public void SetUp()
    {
      svc = new Mock<Logic>();
      sut = new HomeController(svc.Object);
    }

    [TestClass]
    public class Index : HomeControllerTests
    {
      [TestMethod]
      public void GetReturnsDefaultValues()
      {
        var result = sut.Index() as ViewResult;

        var model = result.Model as RedirectInfo;
        Assert.IsNotNull(model);
        Assert.IsNull(model.URL);
        Assert.AreEqual("http://www.randomkittengenerator.com/", model.SafeURL);
        Assert.AreEqual(5 * 60, model.TTL);
      }

      [TestMethod]
      public void PostReturnsShortenedUrl()
      {
        svc
          .Setup(it => it.CreateRedirect("http://example.com/", It.IsAny<string>(), It.IsAny<int>()))
          .Returns("123");
        var info = new RedirectInfo { URL = "example.com" };
        var helper = new MvcHelper();
        helper.SetUpController(sut);
        helper.Response.Setup(x => x.ApplyAppPathModifier("/r/123")).Returns("http://localhost/r/123");

        var result = (sut.Index(info) as ContentResult).Content;

        Assert.IsTrue(result.EndsWith("/r/123"), string.Format("Result is [{0}]", result));
      }

      [TestMethod]
      public void PostNormalizesGivenUrl()
      {
        var info = new RedirectInfo { URL = "example.com" };
        var helper = new MvcHelper();
        helper.SetUpController(sut);

        sut.Index(info);

        svc.Verify(it => it.CreateRedirect("http://example.com/", It.IsAny<string>(), It.IsAny<int>()));
      }

      [TestMethod]
      public void PostNormalizesSafeUrl()
      {
        var info = new RedirectInfo { URL = "example.com", SafeURL = "example.com" };
        var helper = new MvcHelper();
        helper.SetUpController(sut);

        sut.Index(info);

        svc.Verify(it => it.CreateRedirect(It.IsAny<string>(), "http://example.com/", It.IsAny<int>()));
      }

      [TestMethod]
      public void PostReturnsValidationErrorIfUrlIsMissing()
      {
        var info = new RedirectInfo();
        var helper = new MvcHelper();
        helper.SetUpController(sut);
        sut.ValidateModel(info);

        sut.Index(info);

        Assert.IsFalse(sut.ModelState.IsValid);
        Assert.AreEqual(1, sut.ModelState["URL"].Errors.Count);
        Assert.AreEqual("Please enter the URL.", sut.ModelState["URL"].Errors[0].ErrorMessage);
      }
    }

    [TestClass]
    public class r : HomeControllerTests
    {
      [TestMethod]
      public void ReturnsRedirectFromService()
      {
        var redirect = new RedirectResult("http://example.com");
        svc
          .Setup(it => it.GetUrl("abc"))
          .Returns(redirect);

        var result = sut.r("abc") as RedirectResult;

        Assert.AreEqual(redirect, result);
      }

      [TestMethod]
      public void Returns404ForUnknownId()
      {
        var result = sut.r("abc") as HttpNotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("Unknown id abc", result.StatusDescription);
      }
    }

    [TestClass]
    public class Stats : HomeControllerTests
    {
      [TestMethod]
      public void ReturnsModelFromService()
      {
        var summaryInfo = new SummaryInfo();
        svc
          .Setup(it => it.GetSummary())
          .Returns(summaryInfo);

        var result = sut.Stats() as ViewResult;
        Assert.AreEqual(summaryInfo, result.Model);
      }
    }

    [TestClass]
    public class Display : HomeControllerTests
    {
      [TestMethod]
      public void GetReturns404IfCalledWithWrongOrNoKey()
      {
        var result = sut.Display("abc") as HttpNotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("The resource cannot be found.", result.StatusDescription);
      }

      [TestMethod]
      public void GetReturnsDisplayModelIfCalledWithCorrectKey()
      {
        var result = sut.Display(Constants.SECRET) as ViewResult;

        Assert.IsNotNull(result.Model);
      }

      [TestMethod]
      public void PostWithoutCorrectKeyReturns404()
      {
        var result = sut.Display("abc", null, null, null) as HttpNotFoundResult;

        Assert.IsNotNull(result);
        Assert.AreEqual("The resource cannot be found.", result.StatusDescription);
      }

      [TestMethod]
      public void PostWithCorrectKeyReturnsModelFromService()
      {
        var model = new PaginatedRecords();
        svc
          .Setup(it => it.GetRecords(null, null, null))
          .Returns(model);

        var result = sut.Display(Constants.SECRET, null, null, null) as ViewResult;

        Assert.AreEqual(model, result.Model);
      }
    }
  }
}