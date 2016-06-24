using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Renfield.Snake
{
  public static class Events
  {
    public static bool Async { get; set; }

    public static void Subscribe(EventType ev, Action<object, EventArgs> handler)
    {
      Subscribers(ev).Add(handler);
    }

    public static void Unsubscribe(EventType ev, Action<object, EventArgs> handler)
    {
      Subscribers(ev).Remove(handler);
    }

    public static void Raise(EventType ev, object sender, EventArgs args = null)
    {
      if (Async)
        Parallel.ForEach(Subscribers(ev),
          action => action(sender, args));
      else
        foreach (var action in Subscribers(ev))
          action.Invoke(sender, args);
    }

    //

    private static readonly Dictionary<EventType, List<Action<object, EventArgs>>> subscribers = new Dictionary<EventType, List<Action<object, EventArgs>>>();

    private static List<Action<object, EventArgs>> Subscribers(EventType ev)
    {
      if (!subscribers.ContainsKey(ev))
        subscribers.Add(ev, new List<Action<object, EventArgs>>());

      return subscribers[ev];
    }
  }
}