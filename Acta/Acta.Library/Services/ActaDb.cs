using System;
using System.Collections.Generic;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;

namespace Acta.Library.Services
{
  public class ActaDb : IActaLowLevelApi
  {
    public ActaDb(IActaStorage storage)
    {
      this.storage = storage;
    }

    public void Write(Guid guid, string name, object value)
    {
      storage.Append(new ActaTuple(guid, name, value));
    }

    public void Write(Guid guid, params ActaKeyValuePair[] pairs)
    {
      foreach (var pair in pairs)
        Write(guid, pair.Name, pair.Value);
    }

    public IEnumerable<Guid> GetIds(string name, object value)
    {
      return Enumerable.Empty<Guid>();
    }

    public object Read(Guid guid, string name)
    {
      return null;
    }

    //

    private readonly IActaStorage storage;
  }
}