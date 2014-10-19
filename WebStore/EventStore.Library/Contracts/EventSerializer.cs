namespace EventStore.Library.Contracts
{
  public interface EventSerializer
  {
    string Serialize(Event ev);
    Event Deserialize(string value);
  }
}