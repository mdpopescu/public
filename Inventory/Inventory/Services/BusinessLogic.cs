using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Renfield.Inventory.Data;
using Renfield.Inventory.Helpers;
using Renfield.Inventory.Models;

namespace Renfield.Inventory.Services
{
  public class BusinessLogic : Logic
  {
    public BusinessLogic(Func<Repository> dbFactory)
    {
      this.dbFactory = dbFactory;
    }

    public IEnumerable<StockModel> GetStocks()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .Stocks
          .Select(StockModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionModel> GetAcquisitions()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .Acquisitions
          .Include(it => it.Company)
          .Include(it => it.Items.Select(itt => itt.Product))
          .Select(AcquisitionModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionItemModel> GetAcquisitionItems(int id)
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .AcquisitionItems
          .Where(ai => ai.AcquisitionId == id)
          .Include(it => it.Product)
          .Select(AcquisitionItemModel.From)
          .ToList();
    }

    public void AddAcquisition(AcquisitionModel model)
    {
      using (var repository = dbFactory.Invoke())
      using (var transaction = repository.CreateTransaction())
      {
        var productNames = model
          .Items
          .Select(it => it.ProductName)
          .Where(it => !it.IsNullOrEmpty())
          .ToList();
        var products = repository
          .Products
          .Where(it => productNames.Contains(it.Name))
          .ToList();

        var acquisition = ToEntity(repository, products, model);
        if (acquisition == null)
          return;

        repository.Acquisitions.Add(acquisition);
        repository.SaveChanges(); // this updates the ProductId field on the items

        // get the stock records for these products
        var stocks = GetStocks(repository.Stocks, acquisition.Items).ToList();
        foreach (var acquisitionItem in acquisition.Items)
        {
          var item = acquisitionItem;
          Retry.Times(3, TimeSpan.FromMilliseconds(500), () => UpdateStock(repository, stocks, item));
        }
        repository.SaveChanges();

        transaction.Commit();

        UpdateAllClients();
      }
    }

    public IEnumerable<SaleModel> GetSales()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .Sales
          .Include(it => it.Company)
          .Include(it => it.Items.Select(itt => itt.Product))
          .Select(SaleModel.From)
          .ToList();
    }

    public IEnumerable<SaleItemModel> GetSaleItems(int id)
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .SaleItems
          .Where(ai => ai.SaleId == id)
          .Include(it => it.Product)
          .Select(SaleItemModel.From)
          .ToList();
    }

    public void AddSale(SaleModel model)
    {
      using (var repository = dbFactory.Invoke())
      using (var transaction = repository.CreateTransaction())
      {
        var productNames = model
          .Items
          .Select(it => it.ProductName)
          .Where(it => !it.IsNullOrEmpty())
          .ToList();
        var products = repository
          .Products
          .Where(it => productNames.Contains(it.Name))
          .ToList();

        var sale = ToEntity(repository, products, model);
        if (sale == null)
          return;

        repository.Sales.Add(sale);
        repository.SaveChanges(); // this updates the ProductId field on the items

        // get the stock records for these products
        var stocks = GetStocks(repository.Stocks, sale.Items).ToList();
        foreach (var saleItem in sale.Items)
        {
          var item = saleItem;
          Retry.Times(3, TimeSpan.FromMilliseconds(500), () => UpdateStock(repository, stocks, item));
        }
        repository.SaveChanges();

        transaction.Commit();

        UpdateAllClients();
      }
    }

    //

    private readonly Func<Repository> dbFactory;

    private static Acquisition ToEntity(Repository repository, IEnumerable<Product> products, AcquisitionModel model)
    {
      var items = model
        .Items
        .Select(it => ToEntity(repository, products, it))
        .Where(it => it != null)
        .ToList();
      if (!items.Any())
        return null;

      return new Acquisition
      {
        Company = repository.Companies.FirstOrDefault(it => it.Name == model.CompanyName)
                  ?? repository.Companies.Add(new Company {Name = model.CompanyName}),
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = items,
      };
    }

    private static AcquisitionItem ToEntity(Repository repository, IEnumerable<Product> products,
                                            AcquisitionItemModel model)
    {
      if (!model.IsValid())
        return null;

      return new AcquisitionItem
      {
        Product = products.FirstOrDefault(it => it.Name == model.ProductName)
                  ?? repository.Products.Add(new Product {Name = model.ProductName}),
        Quantity = decimal.Parse(model.Quantity),
        Price = decimal.Parse(model.Price),
      };
    }

    private static void UpdateStock(Repository repository, IEnumerable<Stock> stocks, AcquisitionItem acquisitionItem)
    {
      var newQuantity = acquisitionItem.Quantity;
      var product = acquisitionItem.Product;
      var productId = product.Id;

      var stock = stocks.FirstOrDefault(it => it.ProductId == productId);
      if (stock != null)
        stock.Quantity += newQuantity;
      else
      {
        stock = new Stock
        {
          ProductId = productId,
          Name = product.Name,
          SalePrice = product.SalePrice,
          Quantity = newQuantity,
          PurchaseValue = Math.Round(newQuantity * acquisitionItem.Price, 2),
          SaleValue = Math.Round(newQuantity * product.SalePrice.GetValueOrDefault(), 2),
        };
        repository.Stocks.Add(stock);
      }

      repository.SaveChanges();
    }

    private static Sale ToEntity(Repository repository, IEnumerable<Product> products, SaleModel model)
    {
      var items = model
        .Items
        .Select(it => ToEntity(repository, products, it))
        .Where(it => it != null)
        .ToList();
      if (!items.Any())
        return null;

      return new Sale
      {
        Company = repository.Companies.FirstOrDefault(it => it.Name == model.CompanyName)
                  ?? repository.Companies.Add(new Company {Name = model.CompanyName}),
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = items,
      };
    }

    private static SaleItem ToEntity(Repository repository, IEnumerable<Product> products, SaleItemModel model)
    {
      if (!model.IsValid())
        return null;

      return new SaleItem
      {
        Product = products.FirstOrDefault(it => it.Name == model.ProductName)
                  ?? repository.Products.Add(new Product {Name = model.ProductName}),
        Quantity = decimal.Parse(model.Quantity),
        Price = decimal.Parse(model.Price),
      };
    }

    private static void UpdateStock(Repository repository, IEnumerable<Stock> stocks, SaleItem saleItem)
    {
      var newQuantity = saleItem.Quantity;
      var product = saleItem.Product;
      var productId = product.Id;
      var productName = product.Name;

      var stock = stocks.FirstOrDefault(it => it.ProductId == productId);
      if (stock == null)
        throw new Exception(string.Format("Unknown product [{0}]", productName));
      if (stock.Quantity < newQuantity)
        throw new Exception(string.Format("Insufficient quantity for product [{0}]", productName));

      stock.Quantity -= newQuantity;
      repository.SaveChanges();
    }

    private static IEnumerable<Stock> GetStocks<T>(IEnumerable<Stock> stocks, IEnumerable<T> items)
      where T : Item
    {
      var productIds = items
        .Select(it => it.ProductId)
        .ToList();

      return stocks
        .Where(it => productIds.Contains(it.ProductId))
        .ToList();
    }

    private static void UpdateAllClients()
    {
      LiveUpdateHub.Instance.Value.All.updateStocks();
    }
  }
}