using System;

namespace CQRS3.Library.Contracts
{
    public abstract class EventBase
    {
        public Guid Id { get; }
        public DateTimeOffset TimeStamp { get; }

        //

        protected EventBase()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTimeOffset.UtcNow;
        }
    }
}