using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Func<Repository> dbFactory)
    {
      this.dbFactory = dbFactory;
    }

    public IEnumerable<Stock> GetStocks()
    {
      using (var repository = dbFactory.Invoke())
        return repository.Stocks.ToList();
    }

    public IEnumerable<Acquisition> GetAcquisitions()
    {
      using (var repository = dbFactory.Invoke())
        return repository.Acquisitions.Include("Company").ToList();
    }

    public IEnumerable<AcquisitionItem> GetAcquisitionItems(int id)
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .AcquisitionItems
          .Include("Product")
          .Where(ai => ai.AcquisitionId == id)
          .ToList();
    }

    //

    private readonly Func<Repository> dbFactory;
  }
}