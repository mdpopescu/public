using System;
using System.Collections.Generic;
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
          .GetStocks()
          .Select(StockModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionModel> GetAcquisitions()
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .GetAcquisitions()
          .Select(AcquisitionModel.From)
          .ToList();
    }

    public IEnumerable<AcquisitionItemModel> GetAcquisitionItems(int id)
    {
      using (var repository = dbFactory.Invoke())
        return repository
          .GetAcquisitionItems(id)
          .Select(AcquisitionItemModel.From)
          .ToList();
    }

    public void AddAcquisition(AcquisitionModel model)
    {
      using (var repository = dbFactory.Invoke())
      using (var t = repository.BeginTransaction())
      {
        var acquisition = ToEntity(repository, model);
        repository.AddAcquisition(acquisition);

        foreach (var acquisitionItem in acquisition.Items)
          UpdateStock(repository, acquisitionItem);

        repository.SaveChanges();

        // verification phase: if any of the stock records have a different quantity than it should, throw an exception
        // t will be null for testing
        if (t != null)
        {
          var concurrencyViolations = from acquisitionItem in acquisition.Items
                                      let stock = repository.GetStock(acquisitionItem.ProductId)
                                      where stock.Quantity != acquisitionItem.Quantity
                                      select acquisitionItem;
          if (concurrencyViolations.Any())
            throw new Exception("Someone else has changed one of the products; please try again.");

          t.Commit();
        }
      }
    }

    //

    private readonly Func<Repository> dbFactory;

    private static Acquisition ToEntity(Repository repository, AcquisitionModel model)
    {
      return new Acquisition
      {
        Company = repository.FindOrAddCompanyByName(model.CompanyName),
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = model.Items.Select(it => ToEntity(repository, it)).ToList(),
      };
    }

    private static AcquisitionItem ToEntity(Repository repository, AcquisitionItemModel model)
    {
      return new AcquisitionItem
      {
        Product = repository.FindOrAddProductByName(model.ProductName),
        Quantity = decimal.Parse(model.Quantity),
        Price = decimal.Parse(model.Price),
      };
    }

    private static void UpdateStock(Repository repository, AcquisitionItem acquisitionItem)
    {
      var newQuantity = acquisitionItem.Quantity;
      var product = acquisitionItem.Product;
      var productId = product.Id;

      var stock = repository.GetStock(productId);
      if (stock != null)
        repository.UpdateStock(productId, newQuantity + stock.Quantity, stock.Quantity);
      else
      {
        repository.AddStock(new Stock
        {
          ProductId = productId,
          Name = product.Name,
          SalePrice = product.SalePrice,
          Quantity = newQuantity,
          PurchaseValue = Math.Round(newQuantity * acquisitionItem.Price, 2),
          SaleValue = Math.Round(newQuantity * product.SalePrice.GetValueOrDefault(), 2),
        });
      }
    }
  }
}