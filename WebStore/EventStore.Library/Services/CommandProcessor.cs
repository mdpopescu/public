using System;
using System.Collections.Generic;
using EventStore.Library.Contracts;
using EventStore.Library.Models;

namespace EventStore.Library.Services
{
  public class CommandProcessor : Processor<Command>
  {
    public void Register<T>(Func<Command, Event> func)
      where T : Command
    {
      handlers[typeof (T)] = func;
    }

    public void Process(Command command)
    {
      handlers[command.GetType()](command);
    }

    //

    private readonly Dictionary<Type, Func<Command, Event>> handlers = new Dictionary<Type, Func<Command, Event>>();
  }
}