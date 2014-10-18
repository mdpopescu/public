namespace WebStore.Tests.Models
{
  public class Product
  {
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public decimal Quantity { get; private set; }

    public Product(string name, decimal price)
    {
      Name = name;
      Price = price;
      Quantity = 0;
    }

    public void IncQuantity(decimal diff)
    {
      lock (gate)
      {
        Quantity += diff;
      }
    }

    public void DecQuantity(decimal diff)
    {
      lock (gate)
      {
        Quantity -= diff;
      }
    }

    //

    private readonly object gate = new object();
  }
}