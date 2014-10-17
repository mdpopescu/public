using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models.Events
{
  public class ProductCreatedEvent : Event
  {
    public ProductCreatedEvent(string name, decimal price)
    {
      this.name = name;
      this.price = price;
    }

    public Unit Handle(Repository repository)
    {
      var product = new Product { Name = name, Price = price };
      repository.Add(product);

      return Unit.Default;
    }

    private readonly string name;
    private readonly decimal price;
  }
}