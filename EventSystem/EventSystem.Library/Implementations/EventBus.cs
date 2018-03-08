using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace EventSystem.Library.Implementations
{
    public static class EventBus
    {
        static EventBus()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Timer(Broadcast, null, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(10));
        }

        public static void Publish(object obj) => queue.Enqueue(obj);

        public static void AddListener(Action<object> callback)
        {
            lock (callbacksGate)
                callbacks.Add(callback);
        }

        public static void AddListener(Action<object> callback, Func<object, bool> predicate)
        {
            lock (callbacksGate)
                callbacks.Add(
                    item =>
                    {
                        if (predicate(item))
                            callback.Invoke(item);
                    });
        }

        //

        private static readonly ConcurrentQueue<object> queue = new ConcurrentQueue<object>();
        private static readonly List<Action<object>> callbacks = new List<Action<object>>();
        private static readonly object callbacksGate = new object();

        private static void Broadcast(object _)
        {
            do
            {
                if (!queue.TryDequeue(out var item))
                    return;

                SendToAllListeners(item);
            }
            while (true);
        }

        private static void SendToAllListeners(object item)
        {
            lock (callbacksGate)
                foreach (var callback in callbacks)
                    callback.Invoke(item);
        }
    }
}