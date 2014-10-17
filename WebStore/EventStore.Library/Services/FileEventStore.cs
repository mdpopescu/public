using System;
using System.Collections.Generic;
using EventStore.Library.Contracts;

namespace EventStore.Library.Services
{
  public class FileEventStore : AppendOnlyCollection<Event>
  {
    public IEnumerable<Event> Get()
    {
      throw new NotImplementedException();
    }

    public void Append(Event value)
    {
      throw new NotImplementedException();
    }
  }
}