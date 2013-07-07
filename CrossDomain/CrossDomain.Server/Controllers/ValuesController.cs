using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using Renfield.CrossDomain.Server.Models;

namespace Renfield.CrossDomain.Server.Controllers
{
  public class ValuesController : ApiController
  {
    // GET api/values
    [HttpOptions]
    [HttpGet]
    public IEnumerable<string> Get()
    {
      return new[] { "value1", "value2" };
    }

    // GET api/values/5
    public string Get(int id)
    {
      if (User.Identity.Name != "user")
        throw new HttpResponseException(HttpStatusCode.Unauthorized);

      return "value";
    }

    // POST api/values
    public HttpResponseMessage Post(LoginModel model)
    {
      if (model.UserName != "user" || model.Password != "pass")
        throw new HttpResponseException(HttpStatusCode.Unauthorized);

      FormsAuthentication.SetAuthCookie("user", true);
      return new HttpResponseMessage(HttpStatusCode.OK);
    }
  }
}