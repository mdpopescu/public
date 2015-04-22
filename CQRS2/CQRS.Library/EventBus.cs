using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class EventBus
  {
    public static event UnhandledExceptionEventHandler UnhandledException;

    public static IDisposable Subscribe(object obj, params string[] names)
    {
      return new CompositeDisposable(names.Select(name => AddSubscription(obj, name)).ToList());
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

    private static IDisposable AddSubscription(object obj, string name)
    {
      lock (subscriptionsLock)
      {
        var list = subscriptions.GetOrAdd(name, _ => new ConcurrentBag<object>());
        list.Add(obj);
        subscriptions[name] = list;

        return new DisposableGuard(() => Remove(name, obj));
      }
    }

    private static void Remove(string name, object obj)
    {
      // assumes that subscriptions[name] exists, since Remove is only called after it was created
      lock (subscriptionsLock)
      {
        subscriptions[name] = new ConcurrentBag<object>(subscriptions[name].Where(it => it != obj));
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
      else
        WinSystem.Terminate();
    }
  }
}