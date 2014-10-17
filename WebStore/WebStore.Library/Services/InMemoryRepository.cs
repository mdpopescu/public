using System.Collections.Generic;
using System.Linq;
using EventStore.Library.Contracts;

namespace WebStore.Library.Services
{
  public class InMemoryRepository : Repository
  {
    public IEnumerable<T> Get<T>()
      where T : class
    {
      return list.OfType<T>();
    }

    public T GetById<T, TKey>(TKey id)
      where T : class, Entity<TKey>
    {
      return list
        .OfType<T>()
        .Where(it => Equals(it.Id, id))
        .FirstOrDefault();
    }

    public void Add<T>(T entity)
      where T : class
    {
      list.Add(entity);
    }

    public void Update<T>(T entity)
      where T : class
    {
      // do nothing, the object is already in the list
    }

    //

    private readonly List<object> list = new List<object>();
  }
}