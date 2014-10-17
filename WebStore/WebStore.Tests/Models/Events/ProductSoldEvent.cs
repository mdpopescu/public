using System.Linq;
using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models.Events
{
  public class ProductSoldEvent : Event
  {
    public ProductSoldEvent(string name, decimal quantity)
    {
      this.name = name;
      this.quantity = quantity;
    }

    public Unit Handle(Repository repository)
    {
      var existing = repository.Get<Product>().Where(it => it.Name == name).First();
      existing.Quantity -= quantity;
      repository.Update(existing);

      return Unit.Default;
    }

    //

    private readonly string name;
    private readonly decimal quantity;
  }
}