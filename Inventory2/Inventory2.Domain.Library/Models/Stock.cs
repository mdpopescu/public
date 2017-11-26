using Inventory2.Library.Core;

namespace Inventory2.Domain.Library.Models
{
    public class Stock : EntityBase
    {
        public Product Product { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }

        public Stock(Product product, decimal quantity, decimal price)
        {
            Product = product;
            Quantity = quantity;
            Price = price;
        }
    }
}