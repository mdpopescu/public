using System;
using System.Collections.Generic;
using Acta.Library.Contracts;

namespace Acta.Library.Services
{
  public class ActaEntityLayer : ActaEntityApi
  {
    public ActaEntityLayer(ActaLowLevelApi db)
    {
      this.db = db;
    }

    public void AddOrUpdate(object entity)
    {
      db.Write(Guid.NewGuid(), Global.TYPE_KEY, entity.GetType().FullName);
    }

    public Dictionary<string, object> Retrieve(Guid id)
    {
      return new Dictionary<string, object>();
    }

    public T Retrieve<T>(Guid id) where T : class, new()
    {
      return default(T);
    }

    //

    private readonly ActaLowLevelApi db;
  }
}