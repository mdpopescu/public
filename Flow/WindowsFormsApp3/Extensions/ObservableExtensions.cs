using System;
using System.Diagnostics;
using System.Reactive.Linq;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> observable, string label)
        {
#if(TRACE)
            return observable.Do(
                value => Debug.WriteLine($"TRACE: {label} = {value}"),
                e => Debug.WriteLine($"TRACE: {label} exception thrown: {e.Message}"),
                () => Debug.WriteLine($"TRACE: {label} COMPLETED."));
#else
            return observable;
#endif
        }

        public static IObservable<T> MakeWarm<T>(this IObservable<T> cold) =>
            cold.Publish().RefCount();

        public static IObservable<TR> Extract<T, TR>(this IObservable<LabeledValue> source, string label, Func<T, object> selector) => source
            .Where(lv => string.Equals(lv.Label, label, StringComparison.OrdinalIgnoreCase))
            .Select(lv => lv.Value)
            .OfType<T>()
            .Select(selector)
            .OfType<TR>();

        public static IObservable<TR> Extract<TR>(this IObservable<LabeledValue> source, string label) => source
            .Where(lv => string.Equals(lv.Label, label, StringComparison.OrdinalIgnoreCase))
            .Select(lv => lv.Value)
            .OfType<TR>();
    }
}