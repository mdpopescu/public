using System.Collections.Generic;
using Renfield.Inventory.Models;

namespace Renfield.Inventory.Services
{
  public interface Logic
  {
    IEnumerable<StockModel> GetStocks();
    IEnumerable<AcquisitionModel> GetAcquisitions();
    IEnumerable<AcquisitionItemModel> GetAcquisitionItems(int id);
    void AddAcquisition(AcquisitionModel model);
  }
}