using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Challenge2.Rx.Helpers
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Share<T>(this IObservable<T> source) =>
            source.Publish().RefCount();

        public static IObservable<U> AsConst<T, U>(this IObservable<T> source, U value) =>
            source.Select(_ => value);

        public static IObservable<Unit> AsUnit<T>(this IObservable<T> source) =>
            source.AsConst(Unit.Default);

        // return all events, preserving the order
        public static IObservable<U> ConcatMap<T, U>(this IObservable<T> source, Func<T, IObservable<U>> selector) =>
            source.Select(selector).Concat();

        // return all events without preserving the order
        public static IObservable<U> MergeMap<T, U>(this IObservable<T> source, Func<T, IObservable<U>> selector) =>
            source.Select(selector).Merge();

        // only return the events from the most recent stream
        public static IObservable<U> SwitchMap<T, U>(this IObservable<T> source, Func<T, IObservable<U>> selector) =>
            source.Select(selector).Switch();

        /// <summary>
        ///     Inverts the value of a boolean on each incoming event.
        /// </summary>
        /// <param name="source">The incoming events.</param>
        /// <param name="startValue">The starting value for the boolean flag.</param>
        /// <returns>A stream of T/F/T (or F/T/F) values with the same timing as the incoming events.</returns>
        public static IObservable<bool> Toggle<T>(this IObservable<T> source, bool startValue) =>
            source.Scan(startValue, (value, _) => !value).StartWith(startValue);

        public static IObservable<T> WhenTrue<T>(this IObservable<bool> source, Func<IObservable<T>> newSource) =>
            source.SwitchMap(flag => flag ? newSource.Invoke().Publish().RefCount() : Observable.Never<T>());

        public static IObservable<Tuple<T1, T2>> CombineLatest<T1, T2>(this IObservable<T1> source1, IObservable<T2> source2) =>
            source1.CombineLatest(source2, Tuple.Create);
    }
}