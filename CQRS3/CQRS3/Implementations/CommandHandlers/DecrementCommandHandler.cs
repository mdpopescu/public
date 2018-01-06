using System.Collections.Generic;
using CQRS3.Implementations.Commands;
using CQRS3.Implementations.Events;
using CQRS3.Implementations.Queries;
using CQRS3.Library.Contracts;

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

        protected override IEnumerable<EventBase> GenerateEvents(Decrement command)
        {
            var value = getValue.Handle(new GetValueQuery());
            yield return value > 0 ? (EventBase) new Decremented() : new NotDecremented();
        }

        //

        private readonly QueryHandler<GetValueQuery, int> getValue;
    }
}