using System;
using System.Collections.Concurrent;
using System.Linq;

namespace CQRS.Library
{
  public static class EventBus
  {
    public static Action<Exception> UnhandledException
    {
      get { return caller.UnhandledException; }
      set { caller.UnhandledException = value; }
    }

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
    private static readonly MethodCaller caller = new MethodCaller();
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
        caller.Call(method, target, args);
    }
  }
}