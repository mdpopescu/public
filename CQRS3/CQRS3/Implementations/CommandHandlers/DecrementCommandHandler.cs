using System;
using System.Collections.Generic;
using CQRS3.Implementations.Commands;
using CQRS3.Implementations.Events;
using CQRS3.Implementations.Queries;
using CQRS3.Library.Contracts;
using CQRS3.Library.Helpers;
using Void = CQRS3.Library.Helpers.Void;

namespace CQRS3.Implementations.CommandHandlers
{
    public class DecrementCommandHandler : CommandHandler<Decrement>
    {
        public DecrementCommandHandler(EventStore eventStore, QueryHandler<GetValueQuery, int> getValue)
            : base(eventStore)
        {
            this.getValue = getValue;
        }

        //

        protected override Result<Void> Validate(Decrement command)
        {
            var value = getValue.Handle(new GetValueQuery());
            return value > 0 ? Void.Singleton : new Result<Void>(new InvalidOperationException("Cannot decrement below zero."));
        }

        protected override IEnumerable<EventBase> GenerateEvents(Decrement command)
        {
            yield return new Decremented();
        }

        //

        private readonly QueryHandler<GetValueQuery, int> getValue;
    }
}