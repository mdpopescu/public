using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Renfield.Inventory.Data;
using Renfield.Inventory.Models;
using Renfield.Inventory.Services;

namespace Renfield.Inventory.Tests.Services
{
  [TestClass]
  public class BusinessLogicTests
  {
    private List<Product> products;
    private List<Company> companies;
    private List<Stock> stocks;
    private Mock<Repository> repository;
    private BusinessLogic sut;

    [TestInitialize]
    public void SetUp()
    {
      products = new List<Product>();
      companies = new List<Company>();
      stocks = new List<Stock>();

      repository = new Mock<Repository>();
      repository.SetUpTable(it => it.Products, products);
      repository.SetUpTable(it => it.Companies, companies);
      repository.SetUpTable(it => it.Stocks, stocks);

      sut = new BusinessLogic(() => repository.Object);
    }

    [TestClass]
    public class GetStocks : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsStockModels()
      {
        stocks.Add(new Stock { Name = "Hammer", Quantity = 1.00m, SalePrice = 3.45m, PurchaseValue = 5.99m, SaleValue = 7.99m });
        stocks.Add(new Stock { Name = "Nails Pack x100", Quantity = 2.00m, SalePrice = null, PurchaseValue = 0.02m, SaleValue = 0.05m });

        var result = sut.GetStocks().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Hammer", result[0].Name);
        Assert.AreEqual("1.00", result[0].Quantity);
        Assert.AreEqual("3.45", result[0].RRP);
        Assert.AreEqual("5.99", result[0].PurchaseValue);
        Assert.AreEqual("7.99", result[0].SaleValue);
        Assert.AreEqual("Nails Pack x100", result[1].Name);
        Assert.AreEqual("2.00", result[1].Quantity);
        Assert.AreEqual("", result[1].RRP);
        Assert.AreEqual("0.02", result[1].PurchaseValue);
        Assert.AreEqual("0.05", result[1].SaleValue);
      }
    }

    [TestClass]
    public class GetAcquisitions : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsAcquisitionModels()
      {
        repository
          .Setup(it => it.GetAcquisitions())
          .Returns(new[]
          {
            new Acquisition
            {
              Company = new Company { Name = "Microsoft" },
              Date = new DateTime(2000, 3, 4),
              Items = new[]
              {
                new AcquisitionItem { Product = new Product { Name = "Hammer" }, Quantity = 1.23m, Price = 4.56m },
                new AcquisitionItem { Product = new Product { Name = "Saw" }, Quantity = 20.00m, Price = 15.99m },
              }
            },
            new Acquisition
            {
              Company = new Company { Name = "Borland" },
              Date = new DateTime(2000, 5, 6),
              Items = new[]
              {
                new AcquisitionItem { Product = new Product { Name = "Saw" }, Quantity = 10, Price = 12.99m },
                new AcquisitionItem { Product = new Product { Name = "Toolkit" }, Quantity = 10, Price = 29.99m },
              }
            },
          });

        var result = sut.GetAcquisitions().ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Microsoft", result[0].CompanyName);
        Assert.AreEqual("03/04/2000", result[0].Date);
        Assert.AreEqual("325.41", result[0].Value);
        Assert.AreEqual("Borland", result[1].CompanyName);
        Assert.AreEqual("05/06/2000", result[1].Date);
        Assert.AreEqual("429.80", result[1].Value);
      }
    }

    [TestClass]
    public class GetAcquisitionItems : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsAcquisitionItemModels()
      {
        repository
          .Setup(it => it.GetAcquisitionItems(1))
          .Returns(new[]
          {
            new AcquisitionItem { Product = new Product { Name = "Hammer" }, Quantity = 1.23m, Price = 4.56m },
            new AcquisitionItem { Product = new Product { Name = "Saw" }, Quantity = 20.00m, Price = 15.99m },
          });

        var result = sut.GetAcquisitionItems(1).ToList();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Hammer", result[0].ProductName);
        Assert.AreEqual("1.23", result[0].Quantity);
        Assert.AreEqual("4.56", result[0].Price);
        Assert.AreEqual("5.61", result[0].Value);
        Assert.AreEqual("Saw", result[1].ProductName);
        Assert.AreEqual("20.00", result[1].Quantity);
        Assert.AreEqual("15.99", result[1].Price);
        Assert.AreEqual("319.80", result[1].Value);
      }
    }

    [TestClass]
    public class AddAcquisition : BusinessLogicTests
    {
      [TestMethod]
      public void AddsTheCorrectValuesToTheRepository()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        products.Add(new Product { Id = 1, Name = "abc" });
        products.Add(new Product { Id = 2, Name = "def" });
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = "abc", Quantity = "1.23", Price = "4" },
            new AcquisitionItemModel { ProductName = "def", Quantity = "5.67", Price = "8" },
          },
        };

        sut.AddAcquisition(model);

        repository.Verify(it => it.AddAcquisition(It.Is<Acquisition>(a => a.Company.Id == 1 &&
                                                                          a.Date == new DateTime(2000, 2, 3) &&
                                                                          a.Items.Count == 2 &&
                                                                          a.Items.First().Product.Id == 1 &&
                                                                          a.Items.First().Quantity == 1.23m &&
                                                                          a.Items.First().Price == 4.00m)));
        repository.Verify(it => it.SaveChanges());
      }

      [TestMethod]
      public void IgnoresItemsWithInvalidFields()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        products.Add(new Product { Id = 4, Name = "d" });
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "1/1/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = null, Quantity = "1", Price = "1" },
            new AcquisitionItemModel { ProductName = "b", Quantity = "", Price = "2" },
            new AcquisitionItemModel { ProductName = "c", Quantity = "3", Price = "" },
            new AcquisitionItemModel { ProductName = "d", Quantity = "4", Price = "4" },
          },
        };

        sut.AddAcquisition(model);

        repository
          .Verify(it => it.AddAcquisition(It.Is<Acquisition>(a => a.Company.Id == 1 &&
                                                                  a.Date == new DateTime(2000, 1, 1) &&
                                                                  a.Items.Count == 1 &&
                                                                  a.Items.First().Product.Id == 4 &&
                                                                  a.Items.First().Quantity == 4.00m &&
                                                                  a.Items.First().Price == 4.00m)));
      }

      [TestMethod]
      public void DoesNotAddTheRootObjectIfAllItemsAreInvalid()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "1/1/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = null, Quantity = "1", Price = "1" },
            new AcquisitionItemModel { ProductName = "b", Quantity = "", Price = "2" },
            new AcquisitionItemModel { ProductName = "c", Quantity = "3", Price = "" },
          },
        };

        sut.AddAcquisition(model);

        repository.Verify(it => it.AddAcquisition(It.IsAny<Acquisition>()), Times.Never());
      }

      [TestMethod]
      public void UpdatesTheStock()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        products.Add(new Product { Id = 1, Name = "abc" });
        products.Add(new Product { Id = 2, Name = "def", SalePrice = 12.34m });
        stocks.Add(new Stock { Id = 1, ProductId = 1, Quantity = 22.35m });
        Acquisition acquisition = null;
        repository
          .Setup(it => it.AddAcquisition(It.IsAny<Acquisition>()))
          .Callback<Acquisition>(it => acquisition = FixItems(it));

        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = "abc", Quantity = "1.23", Price = "4" },
            new AcquisitionItemModel { ProductName = "def", Quantity = "5.67", Price = "8" },
          },
        };

        sut.AddAcquisition(model);

        Assert.AreEqual(2, stocks.Count);
        Assert.AreEqual(23.58m, stocks[0].Quantity);
        var addedStock = stocks[1];
        Assert.AreEqual(2, addedStock.ProductId);
        Assert.AreEqual("def", addedStock.Name);
        Assert.AreEqual(12.34m, addedStock.SalePrice);
        Assert.AreEqual(5.67m, addedStock.Quantity);
        Assert.AreEqual(45.36m, addedStock.PurchaseValue);
        Assert.AreEqual(69.97m, addedStock.SaleValue);
      }

      [TestMethod]
      public void AddsTheMissingProducts()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        products.Add(new Product { Id = 1, Name = "abc", SalePrice = 12.34m });
        stocks.Add(new Stock { Id = 1, ProductId = 1, Quantity = 22.35m });
        Acquisition acquisition = null;
        repository
          .Setup(it => it.AddAcquisition(It.IsAny<Acquisition>()))
          .Callback<Acquisition>(it => acquisition = FixItems(it));

        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = "abc", Quantity = "1.23", Price = "4" },
            new AcquisitionItemModel { ProductName = "def", Quantity = "5.67", Price = "8" },
          },
        };

        sut.AddAcquisition(model);

        Assert.AreEqual(2, products.Count);
        Assert.AreEqual("abc", products[0].Name);
        Assert.AreEqual("def", products[1].Name);
      }

      [TestMethod]
      public void AddsTheCompanyIfNeeded()
      {
        companies.Add(new Company { Id = 1, Name = "Microsoft" });
        var model = new AcquisitionModel
        {
          CompanyName = "Google",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel { ProductName = "abc", Quantity = "1.23", Price = "4" },
            new AcquisitionItemModel { ProductName = "def", Quantity = "5.67", Price = "8" },
          },
        };

        sut.AddAcquisition(model);

        Assert.AreEqual(2,companies.Count);
        Assert.AreEqual("Google",companies[1].Name);
      }

      //

      /// <summary>
      ///   Fixes the ProductId on the items (this is done by SaveChanges in normal execution)
      /// </summary>
      private static Acquisition FixItems(Acquisition acquisition)
      {
        foreach (var item in acquisition.Items)
          item.ProductId = item.Product.Id;

        return acquisition;
      }
    }
  }
}