using System.Linq;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Library.Services;
using WebStore.Tests.Models;
using WebStore.Tests.Models.Commands;
using WebStore.Tests.Services;

namespace WebStore.Tests
{
  [TestClass]
  public class IntegrationTests
  {
    [TestMethod]
    public void FirstRun()
    {
      var i = 1;

      var store = new InMemoryEventStore();
      var repository = new InMemoryRepository();
      var eventProcessor = new EventProcessor(store, repository);
      var commandProcessor = new CommandProcessor(repository, eventProcessor);

      commandProcessor.Process(new CreateProductCommand("prod1", 15.50m));
      commandProcessor.Process(new CreateProductCommand("prod2", 10.00m));

      commandProcessor.Process(new AddInventoryCommand("prod1", 10));
      commandProcessor.Process(new AddInventoryCommand("prod2", 20));

      commandProcessor.Process(new SellCommand("prod1", 5));
      commandProcessor.Process(new SellCommand("prod2", 5));

      var products = repository.Get<Product, int>().ToList();
      Assert.AreEqual(2, products.Count);
      Assert.AreEqual("prod1", products[0].Name);
      Assert.AreEqual(15.50m, products[0].Price);
      Assert.AreEqual(5, products[0].Quantity);
      Assert.AreEqual("prod2", products[1].Name);
      Assert.AreEqual(10.00m, products[1].Price);
      Assert.AreEqual(15, products[1].Quantity);
    }
  }
}