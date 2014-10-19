using System.Collections.Generic;
using System.Linq;
using EventStore.Library.Contracts;
using WebStore.Api.Data;

namespace WebStore.Api.Services
{
  public class DatabaseEventStore : AppendOnlyCollection<Event>
  {
    public DatabaseEventStore(StoreDb db, EventSerializer serializer)
    {
      this.db = db;
      this.serializer = serializer;
    }

    public IEnumerable<Event> Get()
    {
      return db.Events.Select(ev => serializer.Deserialize(ev.Blob));
    }

    public void Append(Event value)
    {
      db.Events.Add(new EventData { Name = value.GetType().FullName, Blob = serializer.Serialize(value) });
    }

    //

    private readonly StoreDb db;
    private readonly EventSerializer serializer;
  }
}