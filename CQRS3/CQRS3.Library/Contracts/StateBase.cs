using CQRS3.Library.Helpers;

namespace CQRS3.Library.Contracts
{
    public abstract class StateBase
    {
        public void Initialize(EventStore eventStore)
        {
            eventStore.Get().ForEach(Play);
            eventStore.Subscribe(Play);
        }

        /// <summary>
        /// Updates the internal state according to the given event, IF relevant.
        /// </summary>
        /// <param name="ev">The event.</param>
        public abstract void Play(EventBase ev);
    }
}