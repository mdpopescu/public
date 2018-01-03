using System.Collections.Generic;
using System.Linq;
using CQRS3.Library.Contracts;

namespace CQRS3.Implementations
{
    public class InMemoryEventStore : EventStore
    {
        public override IEnumerable<EventBase> Get() => events.AsEnumerable();

        //

        protected override void Save(EventBase ev) => events.Add(ev);

        //

        private readonly List<EventBase> events = new List<EventBase>();
    }
}