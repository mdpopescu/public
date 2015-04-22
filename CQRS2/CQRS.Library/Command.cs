using System;
using System.Reflection;

namespace CQRS.Library
{
  public static class Command
  {
    public static void Send(object target, string name, params object[] args)
    {
      var method = FindMethod(target, name);
      if (method != null)
        method.Invoke(target, args);
      else
      {
        method = FindMethod(target, "MethodMissing");
        if (method != null)
          method.Invoke(target, new object[] { name, args });
        else
          throw new MissingMethodException(target.GetType().FullName, name);
      }
    }

    //

    private delegate void MissingMethodDelegate(string name, params object[] args);

    private static MethodInfo FindMethod(object target, string name)
    {
      var type = target.GetType();
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
    }
  }
}