using System.Linq;
using CQRS3.Contracts;
using CQRS3.Implementations.CommandHandlers;
using CQRS3.Implementations.Commands;
using CQRS3.Implementations.Events;
using CQRS3.Implementations.Queries;

namespace CQRS3.Implementations
{
    public class MainLogic
    {
        public MainLogic(MainUI ui)
        {
            this.ui = ui;

            var eventStore = new TextFileEventStore("events.txt");
            state = new Counter();

            incrementCommandHandler = new IncrementCommandHandler(eventStore);
            decrementCommandHandler = new DecrementCommandHandler(eventStore, state);

            state.StateChanged += (_, __) => ShowCurrentValue();

            state.Initialize(eventStore);
        }

        public void Increment()
        {
            var status = incrementCommandHandler.Execute(new Increment()).Match(ex => ex.Message, list => "OK");
            ui.ShowStatus(status);
        }

        public void Decrement()
        {
            var status = decrementCommandHandler
                         .Execute(new Decrement())
                         .Match(ex => ex.Message, list => list.First() is Decremented ? "OK" : "Decrement failed");
            ui.ShowStatus(status);
        }

        //

        private readonly MainUI ui;

        private readonly Counter state;
        private readonly IncrementCommandHandler incrementCommandHandler;
        private readonly DecrementCommandHandler decrementCommandHandler;

        private void ShowCurrentValue()
        {
            ui.ShowValue(state.Handle(new GetValueQuery()));
        }
    }
}