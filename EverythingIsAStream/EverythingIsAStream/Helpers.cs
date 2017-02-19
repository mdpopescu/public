using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace EverythingIsAStream
{
    public static class Helpers
    {
        public static IObservable<T> ToStream<T>(this Func<T> func, IScheduler scheduler)
        {
            return Observable.Create<T>(o =>
            {
                return scheduler.Schedule(action =>
                {
                    try
                    {
                        o.OnNext(func());
                        action();
                    }
                    catch (Exception ex)
                    {
                        o.OnError(ex);
                    }
                });
            });
        }
    }
}