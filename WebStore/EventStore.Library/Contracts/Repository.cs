using System.Collections.Generic;

namespace EventStore.Library.Contracts
{
  public interface Repository
  {
    IEnumerable<T> Get<T, TKey>()
      where T : class, Entity<TKey>;

    T GetById<T, TKey>(TKey id)
      where T : class, Entity<TKey>;

    void Add<T, TKey>(T entity)
      where T : class, Entity<TKey>;

    void Update<T, TKey>(T entity)
      where T : class, Entity<TKey>;
  }
}