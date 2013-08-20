using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using FluentAssertions;
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
    private List<Acquisition> acquisitions;
    private List<AcquisitionItem> acquisitionItems;
    private List<Sale> sales;
    private List<SaleItem> saleItems;
    private List<Stock> stocks;
    private Mock<IDbTransaction> transaction;
    private Mock<Repository> repository;
    private BusinessLogic sut;

    [TestInitialize]
    public void SetUp()
    {
      products = new List<Product>();
      companies = new List<Company>();
      acquisitions = new List<Acquisition>();
      acquisitionItems = new List<AcquisitionItem>();
      sales = new List<Sale>();
      saleItems = new List<SaleItem>();
      stocks = new List<Stock>();

      repository = new Mock<Repository>();
      repository.SetUpTable(it => it.Products, products);
      repository.SetUpTable(it => it.Companies, companies);
      repository.SetUpTable(it => it.Acquisitions, acquisitions);
      repository.SetUpTable(it => it.AcquisitionItems, acquisitionItems);
      repository.SetUpTable(it => it.Sales, sales);
      repository.SetUpTable(it => it.SaleItems, saleItems);
      repository.SetUpTable(it => it.Stocks, stocks);

      repository
        .Setup(it => it.SaveChanges())
        .Callback(() =>
        {
          acquisitions.ForEach(FixItems);
          sales.ForEach(FixItems);
        });
      transaction = new Mock<IDbTransaction>();
      repository
        .Setup(it => it.CreateTransaction())
        .Returns(transaction.Object);

      sut = new BusinessLogic(() => repository.Object);
    }

    [TestClass]
    public class GetStocks : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsStockModels()
      {
        stocks.Add(new Stock
        {
          Name = "Hammer",
          Quantity = 1.00m,
          SalePrice = 3.45m,
          PurchaseValue = 5.99m,
          SaleValue = 7.99m
        });
        stocks.Add(new Stock
        {
          Name = "Nails Pack x100",
          Quantity = 2.00m,
          SalePrice = null,
          PurchaseValue = 0.02m,
          SaleValue = 0.05m
        });

        var result = sut.GetStocks().ToList();

        result.Should().HaveCount(2);
        result[0].ShouldBeEquivalentTo(new StockModel
        {
          Name = "Hammer",
          Quantity = "1.00",
          RRP = "3.45",
          PurchaseValue = "5.99",
          SaleValue = "7.99"
        });
        result[1].ShouldBeEquivalentTo(new StockModel
        {
          Name = "Nails Pack x100",
          Quantity = "2.00",
          RRP = "",
          PurchaseValue = "0.02",
          SaleValue = "0.05"
        });
      }
    }

    [TestClass]
    public class GetAcquisitions : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsAcquisitionModels()
      {
        acquisitions.AddRange(new[]
        {
          new Acquisition
          {
            Company = new Company {Name = "Microsoft"},
            Date = new DateTime(2000, 3, 4),
            Items = new[]
            {
              new AcquisitionItem {Product = new Product {Name = "Hammer"}, Quantity = 1.23m, Price = 4.56m},
              new AcquisitionItem {Product = new Product {Name = "Saw"}, Quantity = 20.00m, Price = 15.99m},
            }
          },
          new Acquisition
          {
            Company = new Company {Name = "Borland"},
            Date = new DateTime(2000, 5, 6),
            Items = new[]
            {
              new AcquisitionItem {Product = new Product {Name = "Saw"}, Quantity = 10, Price = 12.99m},
              new AcquisitionItem {Product = new Product {Name = "Toolkit"}, Quantity = 10, Price = 29.99m},
            }
          },
        });

        var result = sut.GetAcquisitions().ToList();

        result.Should().HaveCount(2);
        result[0].ShouldBeEquivalentTo(
          new AcquisitionModel {CompanyName = "Microsoft", Date = "03/04/2000", Value = "325.41"},
          options => options.Excluding(m => m.Items));
        result[1].ShouldBeEquivalentTo(
          new AcquisitionModel {CompanyName = "Borland", Date = "05/06/2000", Value = "429.80"},
          options => options.Excluding(m => m.Items));
      }
    }

    [TestClass]
    public class GetAcquisitionItems : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsAcquisitionItemModels()
      {
        acquisitionItems.AddRange(new[]
        {
          new AcquisitionItem
          {
            AcquisitionId = 1,
            Product = new Product {Name = "Hammer"},
            Quantity = 1.23m,
            Price = 4.56m
          },
          new AcquisitionItem
          {
            AcquisitionId = 1,
            Product = new Product {Name = "Saw"},
            Quantity = 20.00m,
            Price = 15.99m
          },
        });

        var result = sut.GetAcquisitionItems(1).ToList();

        result.Should().HaveCount(2);
        result[0].ShouldBeEquivalentTo(new AcquisitionItemModel
        {
          ProductName = "Hammer",
          Quantity = "1.23",
          Price = "4.56",
          Value = "5.61"
        });
        result[1].ShouldBeEquivalentTo(new AcquisitionItemModel
        {
          ProductName = "Saw",
          Quantity = "20.00",
          Price = "15.99",
          Value = "319.80"
        });
      }
    }

    [TestClass]
    public class AddAcquisition : BusinessLogicTests
    {
      [TestMethod]
      public void AddsTheCorrectValuesToTheRepository()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc"});
        products.Add(new Product {Id = 2, Name = "def"});
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new AcquisitionItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddAcquisition(model);

        acquisitions.Should().HaveCount(1);
        var acquisition = acquisitions[0];
        acquisition.ShouldBeEquivalentTo(new Acquisition
        {
          Id = 1,
          Company = new Company {Id = 1, Name = "Microsoft"},
          Date = new DateTime(2000, 2, 3),
          Items = new Collection<AcquisitionItem>
          {
            new AcquisitionItem
            {
              Product = new Product {Id = 1, Name = "abc"},
              ProductId = 1,
              Quantity = 1.23m,
              Price = 4.00m,
            },
            new AcquisitionItem
            {
              Product = new Product {Id = 2, Name = "def"},
              ProductId = 2,
              Quantity = 5.67m,
              Price = 8.00m,
            }
          }
        });
        repository.Verify(it => it.SaveChanges());
      }

      [TestMethod]
      public void IgnoresItemsWithInvalidFields()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 4, Name = "d"});
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "1/1/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = null, Quantity = "1", Price = "1"},
            new AcquisitionItemModel {ProductName = "b", Quantity = "", Price = "2"},
            new AcquisitionItemModel {ProductName = "c", Quantity = "3", Price = ""},
            new AcquisitionItemModel {ProductName = "d", Quantity = "4", Price = "4"},
          },
        };

        sut.AddAcquisition(model);

        acquisitions.Should().HaveCount(1);
        var acquisition = acquisitions[0];
        acquisition.ShouldBeEquivalentTo(new Acquisition
        {
          Id = 1,
          Company = new Company {Id = 1, Name = "Microsoft"},
          Date = new DateTime(2000, 1, 1),
          Items = new Collection<AcquisitionItem>
          {
            new AcquisitionItem
            {
              Product = new Product {Id = 4, Name = "d"},
              ProductId = 4,
              Quantity = 4.00m,
              Price = 4.00m,
            },
          }
        });
      }

      [TestMethod]
      public void DoesNotAddTheRootObjectIfAllItemsAreInvalid()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "1/1/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = null, Quantity = "1", Price = "1"},
            new AcquisitionItemModel {ProductName = "b", Quantity = "", Price = "2"},
            new AcquisitionItemModel {ProductName = "c", Quantity = "3", Price = ""},
          },
        };

        sut.AddAcquisition(model);

        acquisitions.Should().BeEmpty();
      }

      [TestMethod]
      public void UpdatesTheStock()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc"});
        products.Add(new Product {Id = 2, Name = "def", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Name = "abc", Quantity = 22.35m});

        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new AcquisitionItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddAcquisition(model);

        stocks.Should().HaveCount(2);
        stocks[0].Quantity.Should().Be(23.58m);
        var addedStock = stocks[1];
        addedStock.ShouldBeEquivalentTo(new Stock
        {
          Id = 2,
          ProductId = 2,
          Name = "def",
          SalePrice = 12.34m,
          Quantity = 5.67m,
          PurchaseValue = 45.36m,
          SaleValue = 69.97m,
        });
      }

      [TestMethod]
      public void AddsTheMissingProducts()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});

        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new AcquisitionItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddAcquisition(model);

        products.Should().HaveCount(2);
        products[0].Name.Should().Be("abc");
        products[1].Name.Should().Be("def");
      }

      [TestMethod]
      public void AddsTheCompanyIfNeeded()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        var model = new AcquisitionModel
        {
          CompanyName = "Google",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new AcquisitionItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddAcquisition(model);

        companies.Should().HaveCount(2);
        companies[1].Name.Should().Be("Google");
      }

      [TestMethod]
      public void CommitsTheTransaction()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc"});
        products.Add(new Product {Id = 2, Name = "def", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Name = "abc", Quantity = 22.35m});

        var model = new AcquisitionModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new AcquisitionItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new AcquisitionItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddAcquisition(model);

        transaction.Verify(it => it.Commit());
      }
    }

    [TestClass]
    public class GetSales : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsSaleModels()
      {
        sales.AddRange(new[]
        {
          new Sale
          {
            Company = new Company {Name = "Microsoft"},
            Date = new DateTime(2000, 3, 4),
            Items = new[]
            {
              new SaleItem {Product = new Product {Name = "Hammer"}, Quantity = 1.23m, Price = 4.56m},
              new SaleItem {Product = new Product {Name = "Saw"}, Quantity = 20.00m, Price = 15.99m},
            }
          },
          new Sale
          {
            Company = new Company {Name = "Borland"},
            Date = new DateTime(2000, 5, 6),
            Items = new[]
            {
              new SaleItem {Product = new Product {Name = "Saw"}, Quantity = 10, Price = 12.99m},
              new SaleItem {Product = new Product {Name = "Toolkit"}, Quantity = 10, Price = 29.99m},
            }
          },
        });

        var result = sut.GetSales().ToList();

        result.Should().HaveCount(2);
        result[0].ShouldBeEquivalentTo(new SaleModel {CompanyName = "Microsoft", Date = "03/04/2000", Value = "325.41"},
          options => options.Excluding(m => m.Items));
        result[1].ShouldBeEquivalentTo(new SaleModel {CompanyName = "Borland", Date = "05/06/2000", Value = "429.80"},
          options => options.Excluding(m => m.Items));
      }
    }

    [TestClass]
    public class GetSaleItems : BusinessLogicTests
    {
      [TestMethod]
      public void ReturnsSaleItemModels()
      {
        saleItems.AddRange(new[]
        {
          new SaleItem {SaleId = 1, Product = new Product {Name = "Hammer"}, Quantity = 1.23m, Price = 4.56m},
          new SaleItem {SaleId = 1, Product = new Product {Name = "Saw"}, Quantity = 20.00m, Price = 15.99m},
        });

        var result = sut.GetSaleItems(1).ToList();

        result.Should().HaveCount(2);
        result[0].ShouldBeEquivalentTo(new SaleItemModel
        {
          ProductName = "Hammer",
          Quantity = "1.23",
          Price = "4.56",
          Value = "5.61"
        });
        result[1].ShouldBeEquivalentTo(new SaleItemModel
        {
          ProductName = "Saw",
          Quantity = "20.00",
          Price = "15.99",
          Value = "319.80"
        });
      }
    }

    [TestClass]
    public class AddSale : BusinessLogicTests
    {
      [TestMethod]
      public void AddsTheCorrectValuesToTheRepository()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc"});
        products.Add(new Product {Id = 2, Name = "def"});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});
        stocks.Add(new Stock {Id = 2, ProductId = 2, Quantity = 10.05m});
        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddSale(model);

        sales.Should().HaveCount(1);
        var sale = sales[0];
        sale.ShouldBeEquivalentTo(new Sale
        {
          Id = 1,
          Company = new Company {Id = 1, Name = "Microsoft"},
          Date = new DateTime(2000, 2, 3),
          Items = new Collection<SaleItem>
          {
            new SaleItem {Product = new Product {Id = 1, Name = "abc"}, ProductId = 1, Quantity = 1.23m, Price = 4.00m,},
            new SaleItem {Product = new Product {Id = 2, Name = "def"}, ProductId = 2, Quantity = 5.67m, Price = 8.00m,}
          }
        });
        repository.Verify(it => it.SaveChanges());
      }

      [TestMethod]
      public void DoesNotAddTheRootObjectIfAllItemsAreInvalid()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "1/1/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = null, Quantity = "1", Price = "1"},
            new SaleItemModel {ProductName = "b", Quantity = "", Price = "2"},
            new SaleItemModel {ProductName = "c", Quantity = "3", Price = ""},
          },
        };

        sut.AddSale(model);

        sales.Should().BeEmpty();
      }

      [TestMethod]
      public void UpdatesTheStock()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 5.50m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Name = "abc", Quantity = 22.35m});
        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "6"},
          },
        };

        sut.AddSale(model);

        stocks.Should().HaveCount(1);
        stocks[0].Quantity.Should().Be(21.12m);
      }

      [TestMethod]
      public void ThrowsIfThereAreMissingProducts()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});

        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        Action Act = () => sut.AddSale(model);
        Act.ShouldThrow<Exception>().WithMessage("Unknown product [def]");
      }

      [TestMethod]
      public void DoesNotCommitTheTransactionIfThereAreMissingProducts()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});

        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        try
        {
          sut.AddSale(model);
        }
        catch (Exception)
        {
          // ignore
        }

        transaction.Verify(it => it.Commit(), Times.Never());
      }

      [TestMethod]
      public void ThrowsIfAnyQuantityIsInsufficient()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 12.34m});
        products.Add(new Product {Id = 2, Name = "def", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});
        stocks.Add(new Stock {Id = 2, ProductId = 2, Quantity = 0.05m});

        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        Action Act = () => sut.AddSale(model);
        Act.ShouldThrow<Exception>().WithMessage("Insufficient quantity for product [def]");
      }

      [TestMethod]
      public void DoesNotCommitTheTransactionIfAnyQuantityIsInsufficient()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 12.34m});
        products.Add(new Product {Id = 2, Name = "def", SalePrice = 12.34m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});
        stocks.Add(new Stock {Id = 2, ProductId = 2, Quantity = 0.05m});

        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        try
        {
          sut.AddSale(model);
        }
        catch (Exception)
        {
          // ignore
        }

        transaction.Verify(it => it.Commit(), Times.Never());
      }

      [TestMethod]
      public void AddsTheCompanyIfNeeded()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 1.23m});
        products.Add(new Product {Id = 2, Name = "def", SalePrice = 1.23m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Quantity = 22.35m});
        stocks.Add(new Stock {Id = 2, ProductId = 2, Quantity = 10.05m});
        var model = new SaleModel
        {
          CompanyName = "Google",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "4"},
            new SaleItemModel {ProductName = "def", Quantity = "5.67", Price = "8"},
          },
        };

        sut.AddSale(model);

        companies.Should().HaveCount(2);
        companies[1].Name.Should().Be("Google");
      }

      [TestMethod]
      public void CommitsTheTransaction()
      {
        companies.Add(new Company {Id = 1, Name = "Microsoft"});
        products.Add(new Product {Id = 1, Name = "abc", SalePrice = 5.50m});
        stocks.Add(new Stock {Id = 1, ProductId = 1, Name = "abc", Quantity = 22.35m});
        var model = new SaleModel
        {
          CompanyName = "Microsoft",
          Date = "2/3/2000",
          Items = new[]
          {
            new SaleItemModel {ProductName = "abc", Quantity = "1.23", Price = "6"},
          },
        };

        sut.AddSale(model);

        transaction.Verify(it => it.Commit());
      }
    }

    //

    /// <summary>
    ///   Fixes the ProductId on the items (this is done by SaveChanges in normal execution)
    /// </summary>
    private static void FixItems(Acquisition acquisition)
    {
      foreach (var item in acquisition.Items)
        item.ProductId = item.Product.Id;
    }

    /// <summary>
    ///   Fixes the ProductId on the items (this is done by SaveChanges in normal execution)
    /// </summary>
    private static void FixItems(Sale sale)
    {
      foreach (var item in sale.Items)
        item.ProductId = item.Product.Id;
    }
  }
}