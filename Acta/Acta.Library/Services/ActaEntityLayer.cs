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
      //
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

    private ActaLowLevelApi db;
  }
}