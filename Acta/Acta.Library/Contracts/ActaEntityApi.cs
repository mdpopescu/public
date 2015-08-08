using System;
using System.Collections.Generic;

namespace Acta.Library.Contracts
{
  public interface ActaEntityApi
  {
    Guid AddOrUpdate(object entity);
    Dictionary<string, object> Retrieve(Guid id);
    T Retrieve<T>(Guid id) where T : class, new();
  }
}