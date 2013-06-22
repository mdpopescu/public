using System.Data.Entity;

namespace Renfield.Inventory.Library.Data
{
  public class InventoryDB : DbContext
  {
    public DbSet<Product> Products { get; set; }
    public DbSet<Company> Companies { get; set; }
  }
}