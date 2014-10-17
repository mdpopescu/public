using System;
using System.Collections.Generic;
using EventStore.Library.Models;

namespace EventStore.Library.Services
{
  public class CommandProcessor
  {
    public void Register<T>(Func<Command, Event> func)
      where T : Command
    {
      dict[typeof (T)] = func;
    }

    public Event Process(Command command)
    {
      return dict[command.GetType()](command);
    }

    //

    private readonly Dictionary<Type, Func<Command, Event>> dict = new Dictionary<Type, Func<Command, Event>>();
  }
}