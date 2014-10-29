using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EventStore.Library.Contracts;
using WebStore.Library.Models;

namespace WebStore.Api.Controllers
{
  public class ProductsController : ApiController
  {
    public ProductsController(Repository repository)
    {
      this.repository = repository;
    }

    // GET: api/Products
    public IEnumerable<Product> Get()
    {
      return repository
        .Get<Product>();
    }

    // GET: api/Products/5
    public Product Get(int id)
    {
      return repository
        .Get<Product>()
        .Where(it => it.Id == id)
        .FirstOrDefault();
    }

    // POST: api/Products
    public void Post([FromBody] string value)
    {
    }

    // PUT: api/Products/5
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE: api/Products/5
    public void Delete(int id)
    {
    }

    //

    private readonly Repository repository;
  }
}