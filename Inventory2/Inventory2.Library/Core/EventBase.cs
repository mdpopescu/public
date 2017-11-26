using System;

namespace Inventory2.Library.Core
{
    public abstract class EventBase
    {
        public Guid EntityId { get; }

        protected EventBase(Guid entityId)
        {
            EntityId = entityId;
        }
    }
}