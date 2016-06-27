using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.Library
{
  public static class EventBus
  {
    public static IDisposable Subscribe(object obj, params string[] names)
    {
      return new CompositeDisposable(names.Select(name => AddSubscription(obj, name)).ToList());
    }

    public static void Send(string name, params object[] args)
    {
      HashSet<object> list;
      lock (subscriptionsLock)
      {
        if (!subscriptions.TryGetValue(name, out list))
          return;
      }

      foreach (var target in list)
        Send(target, name, args);
    }

    //

    // ReSharper disable InconsistentNaming
    private static readonly ConcurrentDictionary<string, HashSet<object>> subscriptions = new ConcurrentDictionary<string, HashSet<object>>();
    private static readonly object subscriptionsLock = new object();
    // ReSharper enable InconsistentNaming

    private static IDisposable AddSubscription(object obj, string name)
    {
      lock (subscriptionsLock)
      {
        var list = subscriptions.GetOrAdd(name, _ => new HashSet<object>());
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
        subscriptions[name].Remove(obj);
      }
    }

    private static void Send(object target, string name, params object[] args)
    {
      var method = target.FindMethod(name);
      if (method != null)
        MethodCaller.CallAsync(method, target, args);
    }
  }
}