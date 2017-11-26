using System.Collections.Generic;
using System.Linq;
using Inventory2.Library.Core;

namespace Inventory2.Library.Shell
{
    public sealed class CompositeState : StateBase
    {
        public CompositeState(IEnumerable<StateBase> states)
        {
            this.states = states.ToList();
        }

        public override void Handle<TEvent>(TEvent ev)
        {
            foreach (var state in states)
                state.Handle(ev);
        }

        //

        private readonly List<StateBase> states;
    }
}