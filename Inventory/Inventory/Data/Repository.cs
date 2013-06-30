using System.Collections.Generic;

namespace Renfield.Inventory.Data
{
  public interface Repository
  {
    int SaveChanges();

    IEnumerable<Stock> GetStock();
  }
}