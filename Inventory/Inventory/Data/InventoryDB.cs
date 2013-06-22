using System.Data.Entity;
using Renfield.Inventory.Models;

namespace Renfield.Inventory.Data
{
  public class InventoryDB : DbContext
  {
    public DbSet<Product> Products { get; set; }
  }
}