using System.Data.Entity;

namespace Renfield.Inventory.Data
{
  public class InventoryDB : DbContext, Repository
  {
    public DbSet<Product> Products { get; set; }
  }
}