using Renfield.Inventory.Data;
using System.Data.Entity.Migrations;

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

      //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
      //  to avoid creating duplicate seed data. E.g.
      //
      //    context.People.AddOrUpdate(
      //      p => p.FullName,
      //      new Person { FullName = "Andrew Peters" },
      //      new Person { FullName = "Brice Lambson" },
      //      new Person { FullName = "Rowan Miller" }
      //    );
      //

      context.Products.AddOrUpdate(
        new Product { Name = "Hammer", SalePrice = 19.99m },
        new Product { Name = "Nail", SalePrice = 0.01m },
        new Product { Name = "Saw", SalePrice = 14.99m }
        );
    }
  }
}
