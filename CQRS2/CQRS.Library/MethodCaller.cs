using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class MethodCaller
  {
    public static Action<Exception> UnhandledException { get; set; }

    public static void Call(MethodInfo method, object target, params object[] args)
    {
      Call(() => method.Invoke(target, args));
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
      if (UnhandledException == null)
        WinSystem.Terminate();
      else
        UnhandledException(ex);
    }
  }
}