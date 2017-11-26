using System;
using System.Collections.Generic;

namespace Inventory2.Library.Core
{
    public abstract class StateBase
    {
        public virtual void Handle<TEvent>(TEvent ev)
            where TEvent : EventBase
        {
            handlers[ev.GetType()].Invoke(ev);
        }

        public void AddHandler<TEvent>(Action<TEvent> handler)
            where TEvent : EventBase
        {
            handlers.Add(typeof(TEvent), ev => handler((TEvent) ev));
        }

        //

        private readonly Dictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
    }
}