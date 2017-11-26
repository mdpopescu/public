using System;

namespace Inventory2.Library.Core
{
    public abstract class EntityBase
    {
        public Guid Id { get; }

        protected EntityBase(Guid id)
        {
            Id = id;
        }

        protected EntityBase()
            : this(Guid.NewGuid())
        {
        }
    }
}