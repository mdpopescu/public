using System.Reactive;
using EventStore.Library.Contracts;

namespace EventStore.Library.Models
{
  public class NullEvent : Event
  {
    public Unit Handle(Repository repository)
    {
      return Unit.Default;
    }
  }
}