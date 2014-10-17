using EventStore.Library.Contracts;

namespace EventStore.Library.Models
{
  public interface Event
  {
    void Handle(Repository repository);
  }
}