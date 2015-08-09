using System;
using System.Collections.Generic;
using System.Linq;
using Acta.Library.Contracts;
using Acta.Library.Models;

namespace Acta.Library.Services
{
  public class ActaDb : ActaLowLevelApi
  {
    public ActaDb(ActaStorage storage)
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
        .Select(it => it.Id)
        .Distinct();
    }

    public object Read(Guid id, string name)
    {
      return storage
        .GetById(id)
        .Where(it => it.NamesMatch(name))
        .OrderByDescending(it => it.Timestamp)
        .Select(it => it.Value)
        .FirstOrDefault();
    }

    public T Read<T>(Guid id, string name)
    {
      var result = Read(id, name);
      return result == null ? default(T) : (T) result;
    }

    public IEnumerable<ActaKeyValuePair> Read(Guid id)
    {
      var temp1 = storage.GetById(id).ToList();
      var temp2 = temp1.GroupBy(it => it.Name).ToList();
      var temp3 = temp2.Select(g => g.OrderByDescending(it => it.Timestamp).First()).ToList();
      var temp4 = temp3.Select(it => new ActaKeyValuePair(it.Name, it.Value)).ToList();

      return temp4;

      //return storage
      //  .GetById(id)
      //  .GroupBy(tuple => tuple.Name)
      //  .Select(g => g.OrderByDescending(it => it.Timestamp).First())
      //  .Select(it => new ActaKeyValuePair(it.Name, it.Value));
    }

    //

    private readonly ActaStorage storage;
  }
}