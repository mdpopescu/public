using System;
using System.Collections.Generic;

namespace Renfield.SimpleViewEngine.Library.Reflection
{
  public static class ReflectionHelper
  {
    public static Func<string, object> GetPropertyFunc(this object model)
    {
      Func<string, object> func = name =>
      {
        var dict = model as IDictionary<string, object>;
        if (dict != null)
          return dict[name];

        var ps = model.GetType().GetProperty(name);
        if (ps == null)
          throw new Exception(string.Format("Unknown property [{0}]", name));

        return ps.GetValue(model);
      };

      return func;
    }
  }
}