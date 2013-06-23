using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace Renfield.SafeRedir.Tests
{
  public class MvcHelper
  {
    public Mock<HttpRequestBase> Request { get; private set; }
    public Mock<HttpResponseBase> Response { get; private set; }

    public void SetUpController(Controller controller)
    {
      var routes = new RouteCollection();
      MvcApplication.RegisterRoutes(routes);

      Request = new Mock<HttpRequestBase>(MockBehavior.Strict);
      Request.SetupGet(x => x.ApplicationPath).Returns("/");
      Request.SetupGet(x => x.Url).Returns(new Uri("http://localhost/", UriKind.Absolute));
      Request.SetupGet(x => x.ServerVariables).Returns(new NameValueCollection());

      Response = new Mock<HttpResponseBase>(MockBehavior.Strict);
      Response.Setup(x => x.ApplyAppPathModifier("/")).Returns("http://localhost/");

      var context = new Mock<HttpContextBase>(MockBehavior.Strict);
      context.SetupGet(x => x.Request).Returns(Request.Object);
      context.SetupGet(x => x.Response).Returns(Response.Object);

      controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
      controller.Url = new UrlHelper(new RequestContext(context.Object, new RouteData()), routes);
    }
  }
}