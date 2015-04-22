using System;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class Command
  {
    public static event UnhandledExceptionEventHandler UnhandledException;

    public static void Send(object target, string name, params object[] args)
    {
      var method = target.FindMethod(name);
      if (method != null)
        Call(() => method.Invoke(target, args));
      else
      {
        method = target.FindMethod("MethodMissing");
        if (method != null)
          Call(() => method.Invoke(target, new object[] { name, args }));
        else
          throw new MissingMethodException(target.GetType().FullName, name);
      }
    }

    //

    private static void Call(Action action)
    {
      Task.Run(() => TryCall(action));
    }

    private static void TryCall(Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        RaiseUnhandledExceptionEvent(ex);
      }
    }

    private static void RaiseUnhandledExceptionEvent(Exception ex)
    {
      var handler = UnhandledException;
      if (handler != null)
        handler(null, new UnhandledExceptionEventArgs(ex, true));
      else
        WinSystem.Terminate();
    }
  }
}