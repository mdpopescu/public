using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Renfield.RecursiveCompare
{
  public class ObjectComparer
  {
    public IEnumerable<Comparison> Compare(object obj1, object obj2)
    {
      return InternalCompare(obj1, obj2, "");
    }

    //

    private static IEnumerable<Comparison> InternalCompare(object obj1, object obj2, string propName)
    {
      if (obj1 == null && obj2 == null)
        return new[] { new Comparison(propName, null, null) };

      if (obj1 != null && obj2 != null && obj1.GetType() != obj2.GetType())
        throw new Exception("Both values must have the same type.");

      var type = obj1 != null ? obj1.GetType() : obj2.GetType();
      var properties = type.GetProperties();

      if (type.IsPrimitive || type == typeof (string) || !properties.Any())
        return new[] { new Comparison(propName, obj1, obj2) };

      if (type.IsArray)
      {
        var result = new List<Comparison>();
        var array1 = obj1 as Array;
        var array2 = obj2 as Array;

        var array1Length = array1 != null ? array1.Length : 0;
        var array2Length = array2 != null ? array2.Length : 0;
        result.Add(new Comparison(propName + ".Length", array1Length, array2Length));

        for (var i = 0; i < Math.Max(array1Length, array2Length); i++)
          result.AddRange(InternalCompare(GetValueAt(array1, i), GetValueAt(array2, i), propName + "[" + i + "]"));

        return result;
      }

      return properties
        .SelectMany(property => InternalCompare(GetValue(property, obj1), GetValue(property, obj2), propName + "." + property.Name));
    }

    private static object GetValue(PropertyInfo property, object obj)
    {
      return obj != null ? property.GetValue(obj, null) : null;
    }

    private static object GetValueAt(Array array, int i)
    {
      return array != null && i < array.Length ? array.GetValue(i) : null;
    }
  }
}