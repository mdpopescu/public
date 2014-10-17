using System;
using System.Collections.Generic;
using EventStore.Library.Contracts;

namespace EventStore.Library.Services
{
  public class CommandProcessor : Processor<Command>
  {
    public CommandProcessor(Repository repository, Processor<Event> next)
    {
      this.repository = repository;
      this.next = next;
    }

    public void Register<T>()
      where T : Command
    {
      handlers[typeof (T)] = cmd => cmd.Handle(repository);
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
    private readonly Repository repository;
    private readonly Processor<Event> next;
  }
}