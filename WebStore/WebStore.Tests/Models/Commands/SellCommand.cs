using System;
using System.Linq;
using EventStore.Library.Contracts;
using WebStore.Tests.Models.Events;

namespace WebStore.Tests.Models.Commands
{
  public class SellCommand : Command
  {
    public SellCommand(string name, decimal quantity)
    {
      this.name = name;
      this.quantity = quantity;
    }

    public Event Handle(Repository repository)
    {
      // the product must exist
      var existing = repository.Get<Product>().Where(it => it.Name == name).FirstOrDefault();
      if (existing == null)
        throw new Exception("The product " + name + " does not exist.");

      // the quantity must be sufficient
      if (existing.Quantity < quantity)
        throw new Exception("Insufficient quantity for product " + name + " cannot sell " + quantity);

      return new ProductSoldEvent(name, quantity);
    }

    //

    private readonly string name;
    private readonly decimal quantity;
  }
}