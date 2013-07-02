using System.Collections.Generic;
using System.Data.Common;
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

    public DbTransaction BeginTransaction()
    {
      return Database.Connection.BeginTransaction();
    }

    public IEnumerable<Stock> GetStocks()
    {
      return Stocks;
    }

    public Stock GetStock(int productId)
    {
      return Stocks
        .Where(it => it.ProductId == productId)
        .FirstOrDefault();
    }

    public void AddStock(Stock stock)
    {
      Stocks.Add(stock);
    }

    public void UpdateStock(int id, decimal newQuantity, decimal oldQuantity)
    {
      // using optimistic concurrency, therefore I want to throw if the record has been changed
      var stock = Stocks.First(it => it.ProductId == id && it.Quantity == oldQuantity);

      stock.Quantity = newQuantity;
      SaveChanges();
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

    public Product FindOrAddProductByName(string name)
    {
      return Products
               .Where(it => it.Name == name)
               .FirstOrDefault()
             ?? new Product { Name = name };
    }
  }
}