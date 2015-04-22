using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public class MethodCaller
  {
    public Action<Exception> UnhandledException { get; set; }

    public void Call(MethodInfo method, object target, params object[] args)
    {
      Call(() => method.Invoke(target, args));
    }

    //

    private void Call(Action action)
    {
      Task.Run(() => TryCall(action));
    }

    private void TryCall(Action action)
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

    private void RaiseUnhandledExceptionEvent(Exception ex)
    {
      if (UnhandledException == null)
        WinSystem.Terminate();
      else
        UnhandledException(ex);
    }
  }
}