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

    public void Process(Command command)
    {
      var ev = command.Handle(repository);
      if (ev != null)
        next.Process(ev);
    }

    //

    private readonly Repository repository;
    private readonly Processor<Event> next;
  }
}