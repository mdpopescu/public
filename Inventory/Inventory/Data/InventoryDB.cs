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

    public InventoryDB(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    public IEnumerable<Stock> GetStock()
    {
      var inputs = AcquisitionItems.GroupBy(ai => ai.ProductId);
      var outputs = SaleItems.GroupBy(si => si.ProductId);
      var io = from aig in inputs
               join sig in outputs
                 on aig.Key equals sig.Key
               select new { ProductId = aig.Key, aig, sig };
      var quantities = from it in io
                       select new
                       {
                         it.ProductId,
                         qty = (it.aig.Sum(ai => (decimal?) ai.Quantity) - it.sig.Sum(si => (decimal?) si.Quantity)) ?? 0,
                       };
      var ioq = from it in io
                join q in quantities
                  on it.ProductId equals q.ProductId
                select new { it.ProductId, it.aig, it.sig, q.qty };

      var result = from it in ioq
                   join p in Products
                     on it.ProductId equals p.Id
                   select new Stock
                   {
                     Product = p,
                     Name = p.Name,
                     Quantity = it.qty,
                     PurchaseValue = it.aig.Select(ai => (decimal?) ai.Quantity * ai.Price).Sum() ?? 0,
                     SaleValue = it.qty * p.SalePrice ?? 0,
                   };

      return result;


      //return from p in Products
      //       let inputs = from ai in AcquisitionItems where ai.ProductId == p.Id select ai
      //       let outputs = from si in SaleItems where si.ProductId == p.Id select si
      //       let qty = inputs.Select(i => (decimal?) i.Quantity).Sum() -
      //                 outputs.Select(o => (decimal?) o.Quantity).Sum()
      //       select new Stock
      //       {
      //         Product = p,
      //         Name = p.Name,
      //         Quantity = qty.GetValueOrDefault(),
      //         PurchaseValue = inputs.Select(it => (decimal?) it.Quantity * it.Price).Sum().GetValueOrDefault(),
      //         SaleValue = (qty * p.SalePrice).GetValueOrDefault(),
      //       };
    }
  }
}