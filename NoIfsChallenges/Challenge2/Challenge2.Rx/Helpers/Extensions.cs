using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Challenge2.Rx.Helpers
{
    public static class Extensions
    {
        public static IObservable<Unit> AsUnit<T>(this IObservable<T> source) => source.Select(_ => Unit.Default);
    }
}