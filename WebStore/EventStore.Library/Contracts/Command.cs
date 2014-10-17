namespace EventStore.Library.Contracts
{
  public interface Command
  {
    Event Handle(Repository repository);
  }
}