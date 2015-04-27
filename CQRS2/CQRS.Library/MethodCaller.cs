using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class MethodCaller
  {
    public static Action<Exception> UnhandledException { get; set; }

    public static Task CallAsync(MethodInfo method, object target, params object[] args)
    {
      return CallActionAsync(() => method.Invoke(target, args));
    }

    public static Task CallActionAsync(Action action)
    {
      return Task.Run(() => TryCall(action));
    }

    //

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
      if (UnhandledException == null)
        WinSystem.Terminate();
      else
        UnhandledException(ex);
    }
  }
}