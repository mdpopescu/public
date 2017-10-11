using System;
using System.Diagnostics;
using System.Reactive.Linq;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Extensions
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

        //public static IObservable<TR> Extract<T, TR>(this IObservable<LabeledValue> source, string label, Func<T, TR> selector) => source
        //    .Where(lv => string.Equals(lv.Label, label, StringComparison.OrdinalIgnoreCase))
        //    .Select(lv => lv.Value)
        //    .OfType<T>()
        //    .Select(selector);

        public static IObservable<TR> Extract<T, TR>(this IObservable<LabeledValue> source, string label, Func<T, object> selector) => source
            .Where(lv => string.Equals(lv.Label, label, StringComparison.OrdinalIgnoreCase))
            .Select(lv => lv.Value)
            .OfType<T>()
            .Select(selector)
            .OfType<TR>();
    }
}