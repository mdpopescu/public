using EventStore.Library.Contracts;
using ServiceStack.Text;

namespace EventStore.Library.Services
{
  public class StringEventSerializer : EventSerializer
  {
    public string Serialize(Event ev)
    {
      return TypeSerializer.SerializeToString(ev);
    }

    public Event Deserialize(string value)
    {
      return TypeSerializer.DeserializeFromString<Event>(value);
    }
  }
}