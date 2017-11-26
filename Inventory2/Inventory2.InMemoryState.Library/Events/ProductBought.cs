using Inventory2.Domain.Library.Models;
using Inventory2.Library.Core;

namespace Inventory2.InMemoryState.Library.Events
{
    public class ProductBought : EventBase
    {
        public Product Product { get; }
        public decimal Quantity { get; }
        public decimal Price { get; }

        public ProductBought(Product product, decimal quantity, decimal price)
        {
            Product = product;
            Quantity = quantity;
            Price = price;
        }
    }
}