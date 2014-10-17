namespace EventStore.Library.Contracts
{
  public interface Command : Handler<Event>
  {
  }
}