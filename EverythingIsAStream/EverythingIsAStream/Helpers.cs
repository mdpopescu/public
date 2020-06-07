using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace EverythingIsAStream
{
    public static class Helpers
    {
        public static IObservable<T> Run<T>(this IScheduler scheduler, Func<T> func) =>
            Observable.Create<T>(
                o => scheduler.Schedule(
                    action =>
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
                    }
                )
            );
    }
}