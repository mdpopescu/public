using System.Collections.Generic;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Services
{
  public class InMemoryEventStore : AppendOnlyCollection<Event>
  {
    public InMemoryEventStore(IEnumerable<Event> events)
    {
      this.events = new List<Event>(events);
    }

    public IEnumerable<Event> Get()
    {
      return events;
    }

    public void Append(Event value)
    {
      events.Add(value);
    }

    //

    private readonly List<Event> events;
  }
}