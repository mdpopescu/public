using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Renfield.Inventory.Data
{
  public interface Repository : IDisposable
  {
    int SaveChanges();
    DbTransaction BeginTransaction();

    IEnumerable<Stock> GetStocks();
    Stock GetStock(int productId);
    void AddStock(Stock stock);
    void UpdateStock(int id, decimal newQuantity, decimal oldQuantity);
    
    IEnumerable<Acquisition> GetAcquisitions();
    IEnumerable<AcquisitionItem> GetAcquisitionItems(int id);
    void AddAcquisition(Acquisition acquisition);
    
    Company FindOrAddCompanyByName(string name);
    
    Product FindOrAddProductByName(string name);
  }
}