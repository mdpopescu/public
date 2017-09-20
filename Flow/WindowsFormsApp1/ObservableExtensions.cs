using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WindowsFormsApp1
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> source, string name, Func<TSource, string> toString = null)
        {
            toString = toString ?? (it => it?.ToString() ?? "null");

            var id = 0;

            return Observable.Create<TSource>(
                observer =>
                {
                    var id1 = ++id;

                    // ReSharper disable once ConvertToLocalFunction
                    Action<string, string> trace = (m, v) => Debug.WriteLine("{0}{1}: {2}({3})", name, id1, m, v);
                    //trace("Subscribe", "");

                    var disposable = source.Subscribe(
                        v =>
                        {
                            trace("OnNext", toString(v));
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

        public static IObservable<T> When<T>(this IObservable<T> observable, IObservable<bool> gate) => gate
            .StartWith(false)
            .Select(enabled => enabled ? observable : Observable.Never<T>())
            .Switch();

        public static IObservable<T> When<T>(this IObservable<T> observable, IObservable<bool> gate, T defaultValue) => gate
            .StartWith(false)
            .Select(enabled => enabled ? observable : observable.Select(_ => defaultValue))
            .Switch();

        public static IObservable<T> Whenever<T, TGate>(this IObservable<T> observable, IObservable<TGate> gate)
        {
            return gate.WithLatestFrom(observable, (_, value) => value);
        }

        /// <remarks>
        /// This method will leak the <c>hot</c> subject.
        /// Nothing I can do about it, the whole point is to have an observable that does NOT shut down when a subscription ends.
        /// </remarks>
        public static IObservable<T> MakeHot<T>(this IObservable<T> cold)
        {
            var hot = new Subject<T>();
            cold.Subscribe(hot);
            return Observable.Create<T>(obs => hot.Subscribe(obs));
        }
    }
}