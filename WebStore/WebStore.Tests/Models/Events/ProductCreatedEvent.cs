using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models.Events
{
  public class ProductCreatedEvent : Event
  {
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Unit Handle(Repository repository)
    {
      var product = new Product(Name, Price);
      repository.Add(product);

      return Unit.Default;
    }
  }
}