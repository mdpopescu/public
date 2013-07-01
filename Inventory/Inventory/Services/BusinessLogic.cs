using System;
using System.Collections.Generic;
using System.Linq;
using Renfield.Inventory.Data;
using Renfield.Inventory.Models;

namespace Renfield.Inventory.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Func<Repository> dbFactory)
    {
      this.dbFactory = dbFactory;
    }

    public IEnumerable<StockModel> GetStocks()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .GetStocks()
          .Select(StockModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionModel> GetAcquisitions()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .GetAcquisitions()
          .Select(AcquisitionModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionItemModel> GetAcquisitionItems(int id)
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .GetAcquisitionItems(id)
          .Select(AcquisitionItemModel.From)
          .ToList();
    }

    //

    private readonly Func<Repository> dbFactory;
  }
}