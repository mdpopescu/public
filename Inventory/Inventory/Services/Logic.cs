using System.Collections.Generic;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Services
{
  public interface Logic
  {
    IEnumerable<Stock> GetStock();
  }
}