using EventStore.Library.Contracts;

namespace EventStore.Library.Models
{
  public class NullEvent : Event
  {
    public void Handle(Repository repository)
    {
      //
    }
  }
}