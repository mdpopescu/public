using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Renfield.Inventory.Data
{
  public class InventoryDB : DbContext, Repository
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Acquisition> Acquisitions { get; set; }
    public DbSet<AcquisitionItem> AcquisitionItems { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    public InventoryDB(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    public IEnumerable<Stock> GetStocks()
    {
      return Stocks;
    }

    public IEnumerable<Acquisition> GetAcquisitions()
    {
      return Acquisitions
        .Include("Company")
        .Include("Items");
    }

    public IEnumerable<AcquisitionItem> GetAcquisitionItems(int id)
    {
      return AcquisitionItems
        .Include("Product")
        .Where(ai => ai.AcquisitionId == id);
    }
  }
}