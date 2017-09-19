using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace WindowsFormsApp1.Shell
{
    public static class ObservableTrace
    {
        public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> source, string name)
        {
            var id = 0;

            return Observable.Create<TSource>(
                observer =>
                {
                    var id1 = ++id;
                    Action<string, object> trace = (m, v) => Debug.WriteLine("{0}{1}: {2}({3})", name, id1, m, v);
                    //trace("Subscribe", "");
                    var disposable = source.Subscribe(
                        v =>
                        {
                            trace("OnNext", v);
                            observer.OnNext(v);
                        },
                        e =>
                        {
                            trace("OnError", "");
                            observer.OnError(e);
                        },
                        () =>
                        {
                            trace("OnCompleted", "");
                            observer.OnCompleted();
                        });
                    return () =>
                    {
                        trace("Dispose", "");
                        disposable.Dispose();
                    };
                });
        }
    }
}