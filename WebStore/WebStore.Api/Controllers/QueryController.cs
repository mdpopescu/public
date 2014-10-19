using System.Web.Http;

namespace WebStore.Api.Controllers
{
  [Authorize]
  public class QueryController : ApiController
  {
    // POST api/query
    public void Post([FromBody] string value)
    {
    }
  }
}