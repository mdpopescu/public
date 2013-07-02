using System;
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
    private Mock<Repository> repository;
    private BusinessLogic sut;

    [TestInitialize]
    public void SetUp()
    {
      repository = new Mock<Repository>();
      sut = new BusinessLogic(() => repository.Object);
    }

    [TestMethod]
    public void GetStocksReturnsStockModels()
    {
      repository
        .Setup(it => it.GetStocks())
        .Returns(new[]
        {
          new Stock { Name = "Hammer", Quantity = 1.00m, SalePrice = 3.45m, PurchaseValue = 5.99m, SaleValue = 7.99m },
          new Stock { Name = "Nails Pack x100", Quantity = 2.00m, SalePrice = null, PurchaseValue = 0.02m, SaleValue = 0.05m },
        });

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

    [TestMethod]
    public void GetAcquisitionsReturnsAcquisitionModels()
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

    [TestMethod]
    public void GetAcquisitionItemsReturnsAcquisitionItemModels()
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

    [TestMethod]
    public void AddAcquisitionAddsTheCorrectValuesToTheRepository()
    {
      repository
        .Setup(it => it.FindOrAddCompanyByName("Microsoft"))
        .Returns(new Company { Id = 1 });
      repository
        .Setup(it => it.FindOrAddProductByName("abc"))
        .Returns(new Product { Id = 1 });
      repository
        .Setup(it => it.FindOrAddProductByName("def"))
        .Returns(new Product { Id = 2 });
      repository
        .Setup(it => it.AddAcquisition(It.Is<Acquisition>(a => a.Company.Id == 1 &&
                                                               a.Date == new DateTime(2000, 2, 3) &&
                                                               a.Items.Count == 2 &&
                                                               a.Items.First().Product.Id == 1 &&
                                                               a.Items.First().Quantity == 1.23m &&
                                                               a.Items.First().Price == 4.00m)))
        .Verifiable();
      repository
        .Setup(it => it.SaveChanges())
        .Verifiable();

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

      repository.Verify();
    }

    [TestMethod]
    public void AddAcquisitionUpdatesTheStock()
    {
      repository
        .Setup(it => it.FindOrAddCompanyByName("Microsoft"))
        .Returns(new Company { Id = 1 });
      repository
        .Setup(it => it.FindOrAddProductByName("abc"))
        .Returns(new Product { Id = 1, Name = "abc" });
      repository
        .Setup(it => it.FindOrAddProductByName("def"))
        .Returns(new Product { Id = 2, Name = "def", SalePrice = 12.34m });
      repository
        .Setup(it => it.GetStock(1))
        .Returns(new Stock { Quantity = 12.35m });
      repository
        .Setup(it => it.GetStock(2))
        .Returns((Stock) null);

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

      repository.Verify(it => it.UpdateStock(1, 13.58m, 12.35m));
      repository.Verify(it => it.AddStock(It.Is<Stock>(s => s.ProductId == 2 &&
                                                            s.Name == "def" &&
                                                            s.SalePrice == 12.34m &&
                                                            s.Quantity == 5.67m &&
                                                            s.PurchaseValue == 45.36m &&
                                                            s.SaleValue == 69.97m)));
    }
  }
}