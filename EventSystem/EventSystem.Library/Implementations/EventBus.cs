using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace EventSystem.Library.Implementations
{
    public static class EventBus
    {
        public static void Publish<T>(T ev) => EVENTS.OnNext(ev);
        public static IObservable<T> Get<T>() => EVENTS.OfType<T>().AsObservable().ObserveOn(Scheduler.Default);

        public static IDisposable AddSource<T>(IObservable<T> source) => source.Subscribe(obj => EVENTS.OnNext(obj));
        public static IDisposable AddTransformation<T, U>(Func<T, U> func) => Get<T>().Subscribe(it => EVENTS.OnNext(func(it)));
        public static IDisposable AddSink<T>(Action<T> action) => Get<T>().Subscribe(action);

        //

        private static readonly Subject<object> EVENTS = new Subject<object>();
    }
}