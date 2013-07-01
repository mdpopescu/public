using System;
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
        a => a.CompanyId,
        new Acquisition
        {
          CompanyId = companies["Acme"],
          Date = new DateTime(2013, 5, 1),
          Items = new[]
          {
            new AcquisitionItem { ProductId = products["Hammer"], Quantity = 20, Price = 5.99m },
            new AcquisitionItem { ProductId = products["Nail Pack x100"], Quantity = 2000, Price = 0.01m },
          }
        },
        new Acquisition
        {
          CompanyId = companies["Hotpoint"],
          Date = new DateTime(2013, 5, 3),
          Items = new[]
          {
            new AcquisitionItem { ProductId = products["Saw"], Quantity = 10, Price = 12.99m },
            new AcquisitionItem { ProductId = products["Toolkit"], Quantity = 10, Price = 29.99m },
          }
        }
        );

      context.Sales.AddOrUpdate(
        s => s.CompanyId,
        new Sale
        {
          CompanyId = companies["Microsoft"],
          Date = new DateTime(2013, 6, 1),
          Items = new[]
          {
            new SaleItem { ProductId = products["Saw"], Quantity = 3, Price = 19.99m },
            new SaleItem { ProductId = products["Nail Pack x100"], Quantity = 30, Price = 0.05m },
            new SaleItem { ProductId = products["Hammer"], Quantity = 1, Price = 10.99m },
          }
        },
        new Sale
        {
          CompanyId = companies["Borland"],
          Date = new DateTime(2013, 6, 4),
          Items = new[]
          {
            new SaleItem { ProductId = products["Nail Pack x100"], Quantity = 150, Price = 0.04m },
            new SaleItem { ProductId = products["Toolkit"], Quantity = 2, Price = 19.99m },
            new SaleItem { ProductId = products["Hammer"], Quantity = 2, Price = 11.99m },
          }
        }
        );

      context.SaveChanges();

      context.Stocks.AddOrUpdate(
        s => s.ProductId,
        new Stock
        {
          ProductId = products["Hammer"],
          SalePrice = 11.99m,
          Name = "Hammer",
          Quantity = 17,
          PurchaseValue = 101.83m,
          SaleValue = 203.83m,
        },
        new Stock
        {
          ProductId = products["Nail Pack x100"],
          SalePrice = 0.05m,
          Name = "Nail Pack x100",
          Quantity = 1820,
          PurchaseValue = 18.20m,
          SaleValue = 91.00m,
        },
        new Stock
        {
          ProductId = products["Saw"],
          SalePrice = 19.99m,
          Name = "Saw",
          Quantity = 7,
          PurchaseValue = 90.93m,
          SaleValue = 339.83m,
        },
        new Stock
        {
          ProductId = products["Toolkit"],
          SalePrice = 39.99m,
          Name = "Toolkit",
          Quantity = 8,
          PurchaseValue = 239.92m,
          SaleValue = 319.92m,
        }
        );

      context.SaveChanges();
    }
  }
}