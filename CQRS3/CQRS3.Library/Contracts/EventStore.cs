using System;
using System.Collections.Generic;

namespace CQRS3.Library.Contracts
{
    public abstract class EventStore
    {
        public void Add(EventBase ev)
        {
            Save(ev);
            handlers.ForEach(h => h.Invoke(ev));
        }

        public abstract IEnumerable<EventBase> Get();

        public void Subscribe(Action<EventBase> handler)
        {
            handlers.Add(handler);
        }

        //

        protected abstract void Save(EventBase ev);

        //

        private readonly List<Action<EventBase>> handlers = new List<Action<EventBase>>();
    }
}