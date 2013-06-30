using System.Data.Entity.Infrastructure;

namespace Renfield.Inventory.Data
{
  public class InventoryDBFactory : IDbContextFactory<InventoryDB>
  {
    public InventoryDB Create()
    {
      return new InventoryDB("InventoryDB");
    }
  }
}