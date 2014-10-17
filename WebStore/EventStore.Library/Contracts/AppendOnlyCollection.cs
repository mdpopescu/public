using System.Collections.Generic;

namespace EventStore.Library.Contracts
{
  public interface AppendOnlyCollection<T>
  {
    IEnumerable<T> Get();
    void Append(T value);
  }
}