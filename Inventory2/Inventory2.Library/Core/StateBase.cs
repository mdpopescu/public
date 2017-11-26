namespace Inventory2.Library.Core
{
    public abstract class StateBase
    {
        public abstract void Handle<TEvent>(TEvent ev)
            where TEvent : EventBase;
    }
}