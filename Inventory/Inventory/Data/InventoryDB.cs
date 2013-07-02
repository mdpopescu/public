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
    public IDbSet<Stock> Stocks { get; set; }

    public InventoryDB(string nameOrConnectionString)
      : base(nameOrConnectionString)
    {
    }

    public void AddStock(Stock stock)
    {
      Stocks.Add(stock);
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

    public Company FindOrAddCompanyByName(string name)
    {
      var company = Companies
                      .Where(it => it.Name == name)
                      .FirstOrDefault()
                    ?? new Company { Name = name };

      return company;
    }

    public IEnumerable<Product> GetProducts()
    {
      return Products;
    }
  }
}