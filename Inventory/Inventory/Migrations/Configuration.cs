using System.Data.Entity.Migrations;
using System.Linq;
using Renfield.Inventory.Data;

namespace Renfield.Inventory.Migrations
{
  internal sealed class Configuration : DbMigrationsConfiguration<InventoryDB>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(InventoryDB context)
    {
      //  This method will be called after migrating to the latest version.

      context.Products.AddOrUpdate(
        p => p.Name,
        new Product { Name = "Hammer", SalePrice = 11.99m, },
        new Product { Name = "Nail Pack x100", SalePrice = 0.05m, },
        new Product { Name = "Saw", SalePrice = 19.99m, },
        new Product { Name = "Toolkit", SalePrice = 39.99m, }
        );
      context.Companies.AddOrUpdate(
        c => c.Name,
        new Company { Name = "Microsoft" },
        new Company { Name = "Borland" },
        new Company { Name = "Acme" },
        new Company { Name = "Hotpoint" }
        );

      context.SaveChanges();

      // get the name -> id mappings
      var products = context
        .Products
        .ToDictionary(p => p.Name, p => p.Id);
      var companies = context
        .Companies
        .ToDictionary(c => c.Name, c => c.Id);

      context.Acquisitions.AddOrUpdate(
        new Acquisition
        {
          CompanyId = companies["Acme"],
          Items = new[]
          {
            new AcquisitionItem { ProductId = products["Hammer"], Quantity = 20, Price = 5.99m },
            new AcquisitionItem { ProductId = products["Nail Pack x100"], Quantity = 2000, Price = 0.01m },
          }
        },
        new Acquisition
        {
          CompanyId = companies["Hotpoint"],
          Items = new[]
          {
            new AcquisitionItem { ProductId = products["Saw"], Quantity = 10, Price = 12.99m },
            new AcquisitionItem { ProductId = products["Toolkit"], Quantity = 10, Price = 29.99m },
          }
        }
        );

      context.SaveChanges();
    }
  }
}