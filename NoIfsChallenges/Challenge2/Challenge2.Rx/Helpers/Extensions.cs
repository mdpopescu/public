using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Challenge2.Rx.Helpers
{
    public static class Extensions
    {
        public static IObservable<Unit> AsUnit<T>(this IObservable<T> source) =>
            source.Select(_ => Unit.Default);

        public static IObservable<U> SelectSwitch<T, U>(this IObservable<T> source, Func<T, IObservable<U>> selector) =>
            source.Select(selector).Switch().Publish().RefCount();

        /// <summary>
        ///     Inverts the value of a boolean on each incoming event.
        /// </summary>
        /// <param name="source">The incoming events.</param>
        /// <param name="startValue">The starting value for the boolean flag.</param>
        /// <returns>A stream of T/F/T (or F/T/F) values with the same timing as the incoming events.</returns>
        public static IObservable<bool> Toggle<T>(this IObservable<T> source, bool startValue) =>
            source.Scan(startValue, (it, _) => !it).StartWith(startValue);

        public static IObservable<T> WhenTrue<T>(this IObservable<bool> source, Func<IObservable<T>> newSource) =>
            source.SelectSwitch(flag => flag ? newSource.Invoke() : Observable.Never<T>());
    }
}