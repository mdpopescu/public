using System;
using System.Collections.Generic;
using Renfield.SimpleViewEngine.Library.Caching;

namespace Renfield.SimpleViewEngine.Library.Reflection
{
  public static class ReflectionHelper
  {
    public static Func<string, string> GetPropertyFunc(this object model)
    {
      Func<string, string> func = name =>
      {
        var dict = model as IDictionary<string, object>;
        if (dict != null)
          return dict[name].ToString();

        var ps = model.GetType().GetProperty(name);
        return ps.GetValue(model).ToString();
      };

      return func.Memoize();
    }
  }
}