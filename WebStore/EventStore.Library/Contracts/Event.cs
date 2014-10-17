namespace EventStore.Library.Contracts
{
  public interface Event
  {
    void Handle(Repository repository);
  }
}