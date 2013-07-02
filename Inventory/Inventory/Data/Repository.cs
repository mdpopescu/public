using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Renfield.Inventory.Data
{
  public interface Repository : IDisposable
  {
    int SaveChanges();

    IDbSet<Stock> Stocks { get; set; }
    void AddStock(Stock stock);

    IEnumerable<Acquisition> GetAcquisitions();
    IEnumerable<AcquisitionItem> GetAcquisitionItems(int id);
    void AddAcquisition(Acquisition acquisition);

    Company FindOrAddCompanyByName(string name);

    IEnumerable<Product> GetProducts();
  }
}