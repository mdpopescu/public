using System;
using System.Collections.Generic;
using System.Linq;
using Inventory2.InMemoryState.Library.Core;
using Inventory2.InMemoryState.Library.Events;
using Inventory2.Library.Core;

namespace Inventory2.InMemoryState.Library.Commands
{
    public class BuyProduct : CommandBase<InventoryState>
    {
        public BuyProduct(Guid entityId, decimal quantity, decimal price)
        {
            this.entityId = entityId;
            this.quantity = quantity;
            this.price = price;
        }

        public override IEnumerable<EventBase> Execute(InventoryState state)
        {
            var product = state.Products.Where(it => it.Id == entityId).First();
            return new[] { new ProductBought(product, quantity, price) };
        }

        //

        private readonly Guid entityId;
        private readonly decimal quantity;
        private readonly decimal price;
    }
}