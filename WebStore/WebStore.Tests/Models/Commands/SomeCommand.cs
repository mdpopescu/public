using System;
using EventStore.Library.Contracts;

namespace WebStore.Tests.Models.Commands
{
  public class SomeCommand : Command
  {
    public SomeCommand(Func<Repository, Event> func)
    {
      this.func = func;
    }

    public Event Handle(Repository repository)
    {
      return func(repository);
    }

    //

    private readonly Func<Repository, Event> func;
  }
}