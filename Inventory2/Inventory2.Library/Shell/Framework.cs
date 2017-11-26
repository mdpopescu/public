using System.Collections.Generic;
using Inventory2.Library.Contracts;
using Inventory2.Library.Core;

namespace Inventory2.Library.Shell
{
    public class Framework<TState>
        where TState : StateBase, new()
    {
        public Framework(WORMStream events, BinarySerializer<EventBase> serializer)
        {
            this.events = events;
            this.serializer = serializer;

            state = new TState();
        }

        // state -> event -> state
        public void Handle<TEvent>(TEvent ev)
            where TEvent : EventBase
        {
            events.Append(serializer.Serialize(ev));

            // update the state based on the new event
            state.Handle(ev);
        }

        // state -> IEnumerable<event>
        public IEnumerable<EventBase> Execute<TCommand>(TCommand command)
            where TCommand : CommandBase
        {
            foreach (var ev in command.Execute(state))
            {
                Handle(ev);
                yield return ev;
            }
        }

        //

        private readonly WORMStream events;
        private readonly BinarySerializer<EventBase> serializer;

        private readonly TState state;
    }
}