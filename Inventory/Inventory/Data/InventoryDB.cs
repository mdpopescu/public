using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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
    }

    public IEnumerable<Acquisition> GetAcquisitions()
    {
      return Acquisitions
        .Include(it => it.Company)
        .Include(it => it.Items.Select(itt => itt.Product));
    }

    public IEnumerable<AcquisitionItem> GetAcquisitionItems(int id)
    {
      return AcquisitionItems
        .Where(ai => ai.AcquisitionId == id)
        .Include(it => it.Product);
    }

    public void AddAcquisition(Acquisition acquisition)
    {
      Acquisitions.Add(acquisition);
    }
  }
}