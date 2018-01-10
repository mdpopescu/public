using CQRS3.Library.Contracts;

namespace CQRS3.Implementations.Events
{
    public class Unchanged : EventBase
    {
        public string Reason { get; set; }
    }
}