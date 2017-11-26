using System.Collections.Generic;

namespace Inventory2.Library.Core
{
    public abstract class CommandBase<TState>
        where TState : StateBase
    {
        public abstract IEnumerable<EventBase> Execute(TState state);
    }
}