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
    public DbSet<Sale> Sales { get; set; }

    public InventoryDB(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    public IEnumerable<Stock> GetStock()
    {
      return from p in Products
             let inputs = GetInputs(p.Id)
             let outputs = GetOutputs(p.Id)
             let qty = inputs.Select(ai => ai.Quantity).Sum() -
                       outputs.Select(ai => ai.Quantity).Sum()
             select new Stock
             {
               Product = p,
               Name = p.Name,
               Quantity = qty,
               PurchaseValue = inputs.Select(ai => ai.Quantity * ai.Price).Sum(),
               SaleValue = (qty * p.SalePrice).GetValueOrDefault(),
             };
    }

    //

    private IEnumerable<AcquisitionItem> GetInputs(int productId)
    {
      return Acquisitions
        .Select(a => a.Items.Where(ai => ai.ProductId == productId))
        .Flatten();
    }

    private IEnumerable<SaleItem> GetOutputs(int productId)
    {
      return Sales
        .Select(a => a.Items.Where(ai => ai.ProductId == productId))
        .Flatten();
    }
  }
}