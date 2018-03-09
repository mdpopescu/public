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

        //

        private static readonly Subject<object> EVENTS = new Subject<object>();
    }
}