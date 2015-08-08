using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Acta.Library.Contracts;
using Acta.Library.Models;

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
      var list = new List<ActaKeyValuePair> {new ActaKeyValuePair(Global.TYPE_KEY, entity.GetType().FullName)};

      var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      list.AddRange(properties.Select(prop => Convert(prop, entity)));

      db.Write(Guid.NewGuid(), list.ToArray());
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

    private static ActaKeyValuePair Convert(PropertyInfo prop, object entity)
    {
      return new ActaKeyValuePair(prop.Name, prop.GetValue(entity));
    }
  }
}