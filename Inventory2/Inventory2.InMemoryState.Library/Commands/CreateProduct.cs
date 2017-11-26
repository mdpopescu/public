using System;
using System.Collections.Generic;
using System.Linq;
using Inventory2.InMemoryState.Library.Core;
using Inventory2.InMemoryState.Library.Events;
using Inventory2.Library.Core;

namespace Inventory2.InMemoryState.Library.Commands
{
    public class CreateProduct : CommandBase<InventoryState>
    {
        public CreateProduct(string name)
        {
            this.name = name;
        }

        public override IEnumerable<EventBase> Execute(InventoryState state)
        {
            // if a product with the same name already exists, do nothing
            return state.Products.Any(it => string.Equals(it.Name, name, StringComparison.OrdinalIgnoreCase))
                ? Enumerable.Empty<EventBase>()
                : new[] { new ProductCreated(name) };
        }

        //

        private readonly string name;
    }
}