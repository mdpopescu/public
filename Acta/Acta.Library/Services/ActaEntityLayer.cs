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

    public Guid AddOrUpdate(object entity)
    {
      if (entity == null)
        return Guid.Empty;

      var list = new List<ActaKeyValuePair> {new ActaKeyValuePair(Global.TYPE_KEY, entity.GetType().FullName)};

      var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      list.AddRange(properties.Select(prop => Convert(prop, entity)));

      var guid = SetEntityId(entity, properties);
      db.Write(guid, list.ToArray());

      return guid;
    }

    public IEnumerable<ActaKeyValuePair> Retrieve(Guid id)
    {
      return db.Read(id);
    }

    public T Retrieve<T>(Guid id) where T : class, new()
    {
      var result = new T();
      var properties = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

      var pairs = Retrieve(id).ToList();
      foreach (var pair in pairs)
      {
        var prop = properties.Where(it => string.Compare(pair.Name, it.Name, StringComparison.InvariantCultureIgnoreCase) == 0).FirstOrDefault();
        if (prop != null)
          prop.SetValue(result, pair.Value);
      }

      return result;
    }

    //

    private readonly ActaLowLevelApi db;

    private static ActaKeyValuePair Convert(PropertyInfo prop, object entity)
    {
      return new ActaKeyValuePair(prop.Name, prop.GetValue(entity));
    }

    private static Guid SetEntityId(object entity, IEnumerable<PropertyInfo> properties)
    {
      var id = properties.Where(it => it.Name == "Id").First();
      var guid = (Guid) id.GetValue(entity);

      if (guid == Guid.Empty)
        guid = Guid.NewGuid();

      id.SetValue(entity, guid);

      return guid;
    }
  }
}