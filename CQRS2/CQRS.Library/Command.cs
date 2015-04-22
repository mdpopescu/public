using System;

namespace CQRS.Library
{
  public static class Command
  {
    public static Action<Exception> UnhandledException
    {
      get { return caller.UnhandledException; }
      set { caller.UnhandledException = value; }
    }

    public static void Send(object target, string name, params object[] args)
    {
      var method = target.FindMethod(name);
      if (method != null)
        caller.Call(method, target, args);
      else
      {
        method = target.FindMethod("MethodMissing");
        if (method != null)
          caller.Call(method, target, new object[] { name, args });
        else
          throw new MissingMethodException(target.GetType().FullName, name);
      }
    }

    //

    // ReSharper disable once InconsistentNaming
    private static readonly MethodCaller caller = new MethodCaller();
  }
}