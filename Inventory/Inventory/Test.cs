using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Renfield.Inventory.Data;
using Renfield.Inventory.Models;

namespace Renfield.Inventory
{
  public class Test
  {
    public void AddAcquisition(AcquisitionModel model)
    {
      var acquisition = ToEntity(model);
      if (acquisition == null)
        return;

      store.ExecuteCommand(commandFactory.AddAcquisition(acquisition));
      store.SaveChanges(); // this updates the ProductId field on the items

      // get the stock records for these products
      var productIds = store
        .ExecuteQuery(queryFactory.GetProductsForItems(acquisition.Items))
        .Select(it => it.Id)
        .ToList();

      var stocks = store.ExecuteQuery(queryFactory.GetStocks(productIds));

      foreach (var acquisitionItem in acquisition.Items)
        UpdateStock(repository, stocks, acquisitionItem);
      repository.SaveChanges();
    }

    private Store store;
    private CommandFactory commandFactory;
    private QueryFactory queryFactory;

    private Acquisition ToEntity(AcquisitionModel model)
    {
      var productNames = model
        .Items
        .Select(it => it.ProductName)
        .Where(it => !it.IsNullOrEmpty())
        .ToList();

      var products = store.ExecuteQuery(queryFactory.FindProductsByName(productNames)).ToList();

      var items = model
        .Items
        .Select(it => ToEntity(products, it))
        .Where(it => it != null)
        .ToList();
      if (!items.Any())
        return null;

      return new Acquisition
      {
        Company = repository.FindOrAddCompanyByName(model.CompanyName),
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = items,
      };
    }

    private static AcquisitionItem ToEntity(IEnumerable<Product> products, AcquisitionItemModel model)
    {
      if (!model.IsValid())
        return null;

      return new AcquisitionItem
      {
        Product = products.Where(it => it.Name == model.ProductName).FirstOrDefault()
                  ?? repository.Products.Add(new Product { Name = model.ProductName }),
        Quantity = decimal.Parse(model.Quantity),
        Price = decimal.Parse(model.Price),
      };
    }
  }
}

public interface CommandFactory
{
  Command AddAcquisition(Acquisition acquisition);
}

public interface QueryFactory
{
  Query<Product> GetProductsForItems(IEnumerable<AcquisitionItem> items);
  Query<Stock> GetStocks(IEnumerable<int> productIds);
  Query<Product> FindProductsByName(IEnumerable<string> productNames);
}

public interface Store : IDisposable
{
  int SaveChanges();

  IEnumerable<T> ExecuteQuery<T>(Query<T> query);
  void ExecuteCommand(Command command);
}

public interface Query<out T>
{
  IEnumerable<T> Get(InventoryDB db);
}

public interface Command
{
  void Execute(InventoryDB db);
}

public class InventoryDB : DbContext, Store
{
  // list tables and stuff

  public IEnumerable<T> ExecuteQuery<T>(Query<T> query)
  {
    return query.Get(this);
  }

  public void ExecuteCommand(Command command)
  {
    command.Execute(this);
  }
}