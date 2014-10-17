using System;
using System.Reactive;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models
{
  public class SomeEvent : Event
  {
    public SomeEvent(Action<Repository> action)
    {
      this.action = action;
    }

    public Unit Handle(Repository repository)
    {
      action(repository);
      return Unit.Default;
    }

    //

    private readonly Action<Repository> action;
  }
}