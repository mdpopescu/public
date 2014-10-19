using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Library.Contracts;
using EventStore.Library.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Library.Services;
using WebStore.Tests.Models;
using WebStore.Tests.Models.Commands;
using WebStore.Tests.Models.Events;
using WebStore.Tests.Services;

namespace WebStore.Tests
{
  [TestClass]
  public class IntegrationTests
  {
    private AppendOnlyCollection<Event> store;
    private Repository repository;
    private EventProcessor eventProcessor;
    private CommandProcessor commandProcessor;

    public void Initialize(IEnumerable<Event> events)
    {
      store = new InMemoryEventStore(events);
      repository = new InMemoryRepository();
      eventProcessor = new EventProcessor(store, repository);
      commandProcessor = new CommandProcessor(repository, eventProcessor);
    }

    [TestMethod]
    public void FirstRun()
    {
      Initialize(Enumerable.Empty<Event>());

      eventProcessor.Start();

      commandProcessor.Process(new CreateProductCommand("prod1", 15.50m));
      commandProcessor.Process(new CreateProductCommand("prod2", 10.00m));

      commandProcessor.Process(new AddInventoryCommand("prod1", 10));
      commandProcessor.Process(new AddInventoryCommand("prod2", 20));

      commandProcessor.Process(new SellCommand("prod1", 5));
      commandProcessor.Process(new SellCommand("prod2", 5));

      var products = repository.Get<Product>().ToList();
      Assert.AreEqual(2, products.Count);
      Assert.AreEqual("prod1", products[0].Name);
      Assert.AreEqual(15.50m, products[0].Price);
      Assert.AreEqual(5.00m, products[0].Quantity);
      Assert.AreEqual("prod2", products[1].Name);
      Assert.AreEqual(10.00m, products[1].Price);
      Assert.AreEqual(15.00m, products[1].Quantity);
    }

    [TestMethod]
    public void SecondRun()
    {
      var events = new List<Event>
      {
        new ProductCreatedEvent { Name = "prod1", Price = 124.35m },
        new ProductAddedEvent { Name = "prod1", Quantity = 14.50m },
        new ProductAddedEvent { Name = "prod1", Quantity = 3.00m },
        new ProductSoldEvent { Name = "prod1", Quantity = 10.00m },
      };
      Initialize(events);

      eventProcessor.Start();

      commandProcessor.Process(new AddInventoryCommand("prod1", 100.00m));
      commandProcessor.Process(new SellCommand("prod1", 20.00m));

      var products = repository.Get<Product>().ToList();
      Assert.AreEqual(1, products.Count);
      Assert.AreEqual("prod1", products[0].Name);
      Assert.AreEqual(124.35m, products[0].Price);
      Assert.AreEqual(87.50m, products[0].Quantity);
    }

    [TestMethod]
    public void ConcurrencyTest()
    {
      Initialize(Enumerable.Empty<Event>());

      eventProcessor.Start();

      commandProcessor.Process(new CreateProductCommand("prod1", 15.50m));

      var tasks = Enumerable
        .Range(1, 50)
        .Select(_ => Task.Run(() =>
        {
          commandProcessor.Process(new AddInventoryCommand("prod1", 10.00m));
          commandProcessor.Process(new SellCommand("prod1", 9.00m));
        }))
        .ToArray();
      Task.WaitAll(tasks);

      var products = repository.Get<Product>().ToList();
      Assert.AreEqual(1, products.Count);
      Assert.AreEqual(50.00m, products[0].Quantity);
    }
  }
}