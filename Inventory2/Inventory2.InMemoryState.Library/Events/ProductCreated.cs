using Inventory2.Library.Core;

namespace Inventory2.InMemoryState.Library.Events
{
    public class ProductCreated : EventBase
    {
        public string Name { get; }

        public ProductCreated(string name)
        {
            Name = name;
        }
    }
}