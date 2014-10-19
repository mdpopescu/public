using System;
using System.Linq;
using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models.Events
{
  public class ProductAddedEvent : Event
  {
    public string Name { get; set; }
    public decimal Quantity { get; set; }

    public Unit Handle(Repository repository)
    {
      var existing = repository.Get<Product>().Where(it => string.Compare(it.Name, Name, StringComparison.InvariantCultureIgnoreCase) == 0).First();
      existing.IncQuantity(Quantity);
      repository.Update(existing);

      return Unit.Default;
    }
  }
}