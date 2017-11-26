using Inventory2.Library.Core;

namespace Inventory2.Domain.Library.Models
{
    public class Product : EntityBase
    {
        public string Name { get; }

        public Product(string name)
        {
            Name = name;
        }
    }
}