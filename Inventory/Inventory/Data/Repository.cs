using System.Data.Entity;

namespace Renfield.Inventory.Data
{
  public interface Repository
  {
    int SaveChanges();

    DbSet<Product> Products { get; set; }
    DbSet<Company> Companies { get; set; }
    DbSet<Acquisition> Acquisitions { get; set; }
    DbSet<AcquisitionItem> AcquisitionItems { get; set; }
    DbSet<Sale> Sales { get; set; }
    DbSet<SaleItem> SaleItems { get; set; }
    DbSet<Stock> Stocks { get; set; }
  }
}