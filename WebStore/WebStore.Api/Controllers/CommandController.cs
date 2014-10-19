using System.Web.Http;

namespace WebStore.Api.Controllers
{
  [Authorize]
  public class CommandController : ApiController
  {
    // POST api/command
    public void Post([FromBody] string value)
    {
    }
  }
}