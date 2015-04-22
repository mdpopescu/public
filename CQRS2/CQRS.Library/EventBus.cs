using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CQRS.Library
{
  public static class EventBus
  {
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

      Parallel.ForEach(list.AsEnumerable(), target => Send(target, name, args));
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
      var method = FindMethod(target, name);
      if (method != null)
        method.Invoke(target, args);
    }

    private static MethodInfo FindMethod(object target, string name)
    {
      var type = target.GetType();
      return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
    }
  }
}