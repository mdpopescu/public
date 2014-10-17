namespace EventStore.Library.Contracts
{
  public interface Handler<out T>
  {
    T Handle(Repository repository);
  }
}