using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models
{
  public class SomeEvent : Event
  {
    public Unit Handle(Repository repository)
    {
      return Unit.Default;
    }
  }
}