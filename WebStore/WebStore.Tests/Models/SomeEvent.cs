using EventStore.Library.Contracts;
using EventStore.Library.Models;

namespace WebStore.Tests.Models
{
  public class SomeEvent : Event
  {
    public void Handle(Repository repository)
    {
      //
    }
  }
}