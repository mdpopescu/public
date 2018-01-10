using System.Collections.Generic;
using CQRS3.Implementations.Commands;
using CQRS3.Implementations.Events;
using CQRS3.Implementations.Queries;
using CQRS3.Library.Contracts;

namespace CQRS3.Implementations.CommandHandlers
{
    public class IncrementCommandHandler : CommandHandler<Increment>
    {
        public IncrementCommandHandler(EventStore eventStore, QueryHandler<GetValueQuery, int> getValue)
            : base(eventStore)
        {
            this.getValue = getValue;
        }

        //

        protected override IEnumerable<EventBase> GenerateEvents(Increment command)
        {
            var value = getValue.Handle(new GetValueQuery());
            yield return value < 10 ? (EventBase) new Incremented() : new Unchanged { Reason = "Cannot increment over 10." };
        }

        //

        private readonly QueryHandler<GetValueQuery, int> getValue;
    }
}