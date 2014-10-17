using System;
using System.Linq;
using EventStore.Library.Contracts;
using WebStore.Tests.Models.Events;

namespace WebStore.Tests.Models.Commands
{
  public class AddInventoryCommand : Command
  {
    public AddInventoryCommand(string name, decimal quantity)
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

      return new ProductAddedEvent(name, quantity);
    }

    //

    private readonly string name;
    private readonly decimal quantity;
  }
}