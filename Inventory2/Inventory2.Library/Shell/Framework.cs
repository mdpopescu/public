﻿using System.Collections.Generic;
using System.Linq;
using Inventory2.Library.Contracts;
using Inventory2.Library.Core;
using Inventory2.Library.Extensions;

namespace Inventory2.Library.Shell
{
    public class Framework<TState>
        where TState : StateBase, new()
    {
        public TState State { get; }

        public Framework(WORMStream stream, BinarySerializer<EventBase> serializer)
        {
            this.stream = stream;
            this.serializer = serializer;

            State = new TState();
        }

        public void Handle<TEvent>(TEvent ev)
            where TEvent : EventBase
        {
            stream.Append(serializer.Serialize(ev));

            // update the state based on the new event
            State.Handle(ev);
        }

        public List<EventBase> Execute<TCommand>(TCommand command)
            where TCommand : CommandBase<TState>
        {
            return command
                .Execute(State)
                .Do(Handle)
                .ToList();
        }

        //

        private readonly WORMStream stream;
        private readonly BinarySerializer<EventBase> serializer;
    }
}