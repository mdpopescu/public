using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class EventBus
  {
    public static event UnhandledExceptionEventHandler UnhandledException;

    public static void Subscribe(object obj, params string[] names)
    {
      foreach (var name in names)
        AddSubscription(obj, name);
    }

    public static void Send(string name, params object[] args)
    {
      ConcurrentBag<object> list;
      if (!subscriptions.TryGetValue(name, out list))
        return;

      foreach (var target in list)
        Send(target, name, args);
    }

    //

    // ReSharper disable InconsistentNaming
    private static readonly ConcurrentDictionary<string, ConcurrentBag<object>> subscriptions = new ConcurrentDictionary<string, ConcurrentBag<object>>();
    private static readonly object subscriptionsLock = new object();
    // ReSharper enable InconsistentNaming

    private static void AddSubscription(object obj, string name)
    {
      lock (subscriptionsLock)
      {
        var list = subscriptions.GetOrAdd(name, _ => new ConcurrentBag<object>());
        list.Add(obj);
        subscriptions[name] = list;
      }
    }

    private static void Send(object target, string name, params object[] args)
    {
      var method = target.FindMethod(name);
      if (method != null)
        Call(() => method.Invoke(target, args));
    }

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
    }
  }
}