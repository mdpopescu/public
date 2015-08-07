using System;

namespace Acta.Library.Contracts
{
  public interface ActaEntityApi
  {
    void AddOrUpdate(object entity);
    T Retrieve<T>(Guid id) where T : class, new();
  }
}