using System.Collections.Generic;

namespace Inventory2.Library.Core
{
    public abstract class CommandBase
    {
        public abstract IEnumerable<EventBase> Execute<TState>(TState state)
            where TState : StateBase;
    }
}