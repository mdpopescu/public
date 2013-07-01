using System;
using System.Collections.Generic;

namespace Renfield.Inventory.Data
{
  public interface Repository : IDisposable
  {
    int SaveChanges();

    IEnumerable<Stock> GetStocks();
    IEnumerable<Acquisition> GetAcquisitions();
    IEnumerable<AcquisitionItem> GetAcquisitionItems(int id);
  }
}