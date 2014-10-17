using System.Reactive;

namespace EventStore.Library.Contracts
{
  public interface Event : Handler<Unit>
  {
  }
}