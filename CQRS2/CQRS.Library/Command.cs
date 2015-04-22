using System;

namespace CQRS.Library
{
  public static class Command
  {
    public static void Send(object target, string name, params object[] args)
    {
      var method = target.FindMethod(name);
      if (method != null)
        MethodCaller.Call(method, target, args);
      else
      {
        method = target.FindMethod("MethodMissing");
        if (method != null)
          MethodCaller.Call(method, target, new object[] { name, args });
        else
          throw new MissingMethodException(target.GetType().FullName, name);
      }
    }
  }
}