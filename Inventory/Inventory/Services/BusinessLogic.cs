using System.Collections.Generic;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Repository repository)
    {
      this.repository = repository;
    }

    public IEnumerable<Stock> GetStock()
    {
      return repository.GetStock();
    }

    //

    private readonly Repository repository;
  }
}