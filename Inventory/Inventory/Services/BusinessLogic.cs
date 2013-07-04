using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Renfield.Inventory.Data;
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
        var productIds = acquisition
          .Items
          .Select(it => it.ProductId)
          .ToList();
        var stocks = repository
          .Stocks
          .Where(it => productIds.Contains(it.ProductId))
          .ToList();

        foreach (var acquisitionItem in acquisition.Items)
          UpdateStock(repository, stocks, acquisitionItem);
        repository.SaveChanges();

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
        Company = repository.Companies.Where(it => it.Name == model.CompanyName).FirstOrDefault()
                  ?? repository.Companies.Add(new Company { Name = model.CompanyName }),
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = items,
      };
    }

    private static AcquisitionItem ToEntity(Repository repository, IEnumerable<Product> products, AcquisitionItemModel model)
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
    }

    private static void UpdateAllClients()
    {
      LiveUpdateHub.Instance.Value.All.updateStocks();
    }
  }
}