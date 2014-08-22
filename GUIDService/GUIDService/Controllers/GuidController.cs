using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Renfield.GUIDService.Controllers
{
  public class GuidController : ApiController
  {
    // GET api/guid
    public string Get()
    {
      return GetGuid();
    }

    // GET api/guid/5
    public IEnumerable<string> Get(int count)
    {
      return Enumerable
        .Range(1, count)
        .Select(_ => GetGuid());
    }

    //

    private static string GetGuid()
    {
      return Guid.NewGuid().ToString("B");
    }
  }
}