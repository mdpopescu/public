using System;
using System.Linq;
using EventStore.Library.Contracts;
using WebStore.Tests.Models.Events;

namespace WebStore.Tests.Models.Commands
{
  public class CreateProductCommand : Command
  {
    public CreateProductCommand(string name, decimal price)
    {
      this.name = name;
      this.price = price;
    }

    public Event Handle(Repository repository)
    {
      // the product must not exist
      var existing = repository.Get<Product>().Where(it => it.Name == name).FirstOrDefault();
      if (existing != null)
        throw new Exception("The product " + name + " already exists.");

      return new ProductCreatedEvent { Name = name, Price = price };
    }

    //

    private readonly string name;
    private readonly decimal price;
  }
}