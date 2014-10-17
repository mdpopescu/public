using EventStore.Library.Contracts;
using EventStore.Library.Models;

namespace EventStore.Library.Services
{
  public class EventProcessor : Processor<Event>
  {
    public EventProcessor(AppendOnlyCollection<Event> store, Repository repository)
    {
      this.store = store;
      this.repository = repository;
    }

    public void Process(Event input)
    {
      //
    }

    public void Start()
    {
      foreach (var ev in store.Get())
      {
        ev.Handle(repository);
      }
    }

    //

    private readonly AppendOnlyCollection<Event> store;
    private readonly Repository repository;
  }
}