using System;
using System.Data.Entity;
using System.Transactions;

namespace Renfield.Inventory.Data
{
  public interface Repository : IDisposable
  {
    IDbSet<Product> Products { get; set; }
    IDbSet<Company> Companies { get; set; }
    IDbSet<Acquisition> Acquisitions { get; set; }
    IDbSet<AcquisitionItem> AcquisitionItems { get; set; }
    IDbSet<Sale> Sales { get; set; }
    IDbSet<SaleItem> SaleItems { get; set; }
    IDbSet<Stock> Stocks { get; set; }

    int SaveChanges();

    TransactionBlock CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
  }
}