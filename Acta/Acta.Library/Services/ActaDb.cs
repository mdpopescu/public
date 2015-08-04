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
      storage.Append(new ActaTuple(guid, name, value, Global.Time()));
    }

    public void Write(Guid guid, params ActaKeyValuePair[] pairs)
    {
      foreach (var pair in pairs)
        Write(guid, pair.Name, pair.Value);
    }

    public IEnumerable<Guid> GetIds(string name, object value)
    {
      return storage
        .Get()
        .Where(it => it.Matches(name, value))
        .Select(it => it.Id);
    }

    public object Read(Guid id, string name)
    {
      return storage
        .Get()
        .Where(it => it.Matches(id, name))
        .Select(it => it.Value)
        .FirstOrDefault();
    }

    public T Read<T>(Guid id, string name)
    {
      return default(T);
    }

    //

    private readonly IActaStorage storage;
  }
}