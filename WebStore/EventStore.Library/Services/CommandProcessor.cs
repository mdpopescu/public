using System;
using System.Collections.Generic;
using EventStore.Library.Contracts;
using EventStore.Library.Models;

namespace EventStore.Library.Services
{
  public class CommandProcessor : Processor<Command>
  {
    public CommandProcessor(Processor<Event> next)
    {
      this.next = next;
    }

    public void Register<T>(Func<Command, Event> func)
      where T : Command
    {
      handlers[typeof (T)] = func;
    }

    public void Process(Command command)
    {
      var key = command.GetType();
      var ev = handlers.ContainsKey(key) ? handlers[key](command) : null;

      if (ev != null)
        next.Process(ev);
    }

    //

    private readonly Dictionary<Type, Func<Command, Event>> handlers = new Dictionary<Type, Func<Command, Event>>();
    private readonly Processor<Event> next;
  }
}