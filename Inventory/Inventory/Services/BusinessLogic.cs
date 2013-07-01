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
      {
        repository.AddAcquisition(ToEntity(repository, model));

        repository.SaveChanges();
      }
    }

    //

    private readonly Func<Repository> dbFactory;

    private static Acquisition ToEntity(Repository repository, AcquisitionModel model)
    {
      return new Acquisition
      {
        CompanyId = repository.FindOrAddCompanyByName(model.CompanyName).Id,
        Date = model.Date.ParseDateNullable() ?? DateTime.Today,
        Items = model.Items.Select(it => ToEntity(repository, it)).ToList(),
      };
    }

    private static AcquisitionItem ToEntity(Repository repository, AcquisitionItemModel model)
    {
      return new AcquisitionItem
      {
        ProductId = repository.FindOrAddProductByName(model.ProductName).Id,
        Quantity = decimal.Parse(model.Quantity),
        Price = decimal.Parse(model.Price),
      };
    }
  }
}