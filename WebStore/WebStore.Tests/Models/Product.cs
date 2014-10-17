using EventStore.Library.Contracts;

namespace WebStore.Tests.Models
{
  public class Product : Entity<int>
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
  }
}