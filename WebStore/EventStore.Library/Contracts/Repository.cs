using System.Collections.Generic;

namespace EventStore.Library.Contracts
{
  public interface Repository
  {
    IEnumerable<T> Get<T>()
      where T : class;

    void Add<T>(T entity)
      where T : class;

    void Update<T>(T entity)
      where T : class;
  }
}