using System.Collections.Generic;
using CQRS3.Implementations.Commands;
using CQRS3.Implementations.Events;
using CQRS3.Library.Contracts;

namespace CQRS3.Implementations.CommandHandlers
{
    public class IncrementCommandHandler : CommandHandler<Increment>
    {
        public IncrementCommandHandler(EventStore eventStore)
            : base(eventStore)
        {
        }

        //

        protected override IEnumerable<EventBase> GenerateEvents(Increment command)
        {
            yield return new Incremented();
        }
    }
}