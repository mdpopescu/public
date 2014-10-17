using System.Collections.Generic;
using System.Linq;
using EventStore.Library.Contracts;

namespace WebStore.Library.Services
{
  public class InMemoryRepository : Repository
  {
    public IEnumerable<T> Get<T, TKey>()
      where T : class, Entity<TKey>
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

    public void Add<T, TKey>(T entity)
      where T : class, Entity<TKey>
    {
      list.Add(entity);
    }

    public void Update<T, TKey>(T entity)
      where T : class, Entity<TKey>
    {
      // do nothing, the object is already in the list
    }

    //

    private readonly List<object> list = new List<object>();
  }
}