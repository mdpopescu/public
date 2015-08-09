using System;
using System.Collections.Generic;
using Acta.Library.Models;

namespace Acta.Library.Contracts
{
  public interface ActaEntityApi
  {
    Guid AddOrUpdate(object entity);
    IEnumerable<ActaKeyValuePair> Retrieve(Guid id);
    T Retrieve<T>(Guid id) where T : class, new();
  }
}