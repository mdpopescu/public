using System;
using System.Data.Entity;

namespace Renfield.Inventory.Data
{
  public interface Repository : IDisposable
  {
    int SaveChanges();

    IDbSet<Product> Products { get; set; }
    IDbSet<Company> Companies { get; set; }
    IDbSet<Acquisition> Acquisitions { get; set; }
    IDbSet<AcquisitionItem> AcquisitionItems { get; set; }
    IDbSet<Sale> Sales { get; set; }
    IDbSet<SaleItem> SaleItems { get; set; }
    IDbSet<Stock> Stocks { get; set; }
  }
}