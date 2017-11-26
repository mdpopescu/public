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
        public BuyProduct(Guid entityId, decimal price, decimal quantity)
        {
            this.entityId = entityId;
            this.price = price;
            this.quantity = quantity;
        }

        public override IEnumerable<EventBase> Execute(InventoryState state)
        {
            var product = state.Products.Where(it => it.Id == entityId).First();
            return new[] { new ProductBought(product, quantity, price) };
        }

        //

        private readonly Guid entityId;
        private readonly decimal price;
        private readonly decimal quantity;
    }
}