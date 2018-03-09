using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EventSystem.Library.Models;

namespace EventSystem.Library.Implementations
{
    public static class EventBus
    {
        public static void Publish(LabeledValue value) => EVENTS.OnNext(value);

        public static IObservable<LabeledValue> Get(string label) =>
            EVENTS
                .Where(it => string.Equals(label, it.Label, StringComparison.CurrentCultureIgnoreCase))
                .AsObservable()
                .ObserveOn(Scheduler.Default);

        public static IDisposable AddSource(IObservable<LabeledValue> source) => source.Subscribe(obj => EVENTS.OnNext(obj));
        public static IDisposable AddTransformation(string label, Func<LabeledValue, LabeledValue> func) => Get(label).Subscribe(it => EVENTS.OnNext(func(it)));
        public static IDisposable AddSink(string label, Action<LabeledValue> action) => Get(label).Subscribe(action);

        //

        private static readonly Subject<LabeledValue> EVENTS = new Subject<LabeledValue>();
    }
}