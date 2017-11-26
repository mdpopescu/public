using System.Collections.Generic;
using Inventory2.Library.Core;

namespace Inventory2.Library.Contracts
{
    public interface EventStore
    {
        void Add(EventBase ev);
        IEnumerable<EventBase> GetAll();
    }
}