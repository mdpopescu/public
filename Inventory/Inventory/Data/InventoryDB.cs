using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Transactions;

namespace Renfield.Inventory.Data
{
  public class InventoryDB : DbContext, Repository
  {
    public IDbSet<Product> Products { get; set; }
    public IDbSet<Company> Companies { get; set; }
    public IDbSet<Acquisition> Acquisitions { get; set; }
    public IDbSet<AcquisitionItem> AcquisitionItems { get; set; }
    public IDbSet<Sale> Sales { get; set; }
    public IDbSet<SaleItem> SaleItems { get; set; }
    public IDbSet<Stock> Stocks { get; set; }

    public InventoryDB(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
      var connection = ((IObjectContextAdapter) this).ObjectContext.Connection;
      connection.Open();
    }

    public TransactionBlock CreateTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
      return new TransactionScopeWrapper(isolationLevel);
    }
  }
}