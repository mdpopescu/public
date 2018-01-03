using System.Collections.Generic;
using System.Linq;
using CQRS3.Library.Helpers;

namespace CQRS3.Library.Contracts
{
    public abstract class CommandHandler<TCommand>
    {
        public Result<IEnumerable<EventBase>> Execute(TCommand command) =>
            Validate(command)
                .Match<Result<IEnumerable<EventBase>>>(ex => ex, _ => GenerateEvents(command).Do(eventStore.Add).ToList());

        //

        protected readonly EventStore eventStore;

        protected CommandHandler(EventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        /// <summary>
        /// Validates the given command.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <returns>The <c>Void.Singleton</c> instance if validation is successful, an exception otherwise.</returns>
        /// <remarks>
        /// 1. The default implementation assumes no validation is necessary and thus returns the "ok" flag.
        /// 2. If validation fails, the exception is returned and not thrown.
        /// </remarks>
        protected virtual Result<Void> Validate(TCommand command) => new Result<Void>(Void.Singleton);

        /// <summary>
        /// Generates the appropriate events for the given command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>The events signifying that the command has been handled.</returns>
        protected abstract IEnumerable<EventBase> GenerateEvents(TCommand command);
    }
}