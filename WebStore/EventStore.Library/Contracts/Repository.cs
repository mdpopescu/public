using System.Collections.Generic;

namespace EventStore.Library.Contracts
{
  public interface Repository
  {
    IEnumerable<T> Get<T>()
      where T : class;

    T GetById<T, TKey>(TKey id)
      where T : class, Entity<TKey>;

    void Add<T>(T entity)
      where T : class;

    void Update<T>(T entity)
      where T : class;
  }
}