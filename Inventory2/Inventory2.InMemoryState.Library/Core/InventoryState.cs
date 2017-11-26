using System.Collections.Generic;
using System.Linq;
using Inventory2.Domain.Library.Models;
using Inventory2.InMemoryState.Library.Events;
using Inventory2.Library.Core;

namespace Inventory2.InMemoryState.Library.Core
{
    public class InventoryState : StateBase
    {
        public IEnumerable<Product> Products => products.AsEnumerable();
        public IEnumerable<Stock> Stock => stock.AsEnumerable();

        public InventoryState()
        {
            AddHandler<ProductCreated>(HandleProductCreated);
            AddHandler<ProductBought>(HandleProductBought);
        }

        //

        private readonly List<Product> products = new List<Product>();
        private readonly List<Stock> stock = new List<Stock>();

        private void HandleProductCreated(ProductCreated ev)
        {
            var product = new Product(ev.Name);
            products.Add(product);
            ev.EntityId = product.Id;
        }

        private void HandleProductBought(ProductBought ev)
        {
            var item = new Stock(ev.Product, ev.Quantity, ev.Price);
            stock.Add(item);
            ev.EntityId = item.Id;
        }
    }
}