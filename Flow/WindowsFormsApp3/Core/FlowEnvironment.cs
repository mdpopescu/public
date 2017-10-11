using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WindowsFormsApp3.Contracts;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Core
{
    public class FlowEnvironment
    {
        public void AddInput<T>(string label, IObservable<T> source) => source
            .Select(value => new LabeledValue(label, value))
            .Subscribe(bus.OnNext);

        public void AddComponent(FlowComponent component) => component
            .Process(bus)
            .Subscribe(bus.OnNext);

        public void AddOutput<T>(string label, IObserver<T> sink) => bus
            .Where(lv => string.Equals(label, lv.Label, StringComparison.OrdinalIgnoreCase))
            .Select(lv => lv.Value)
            .OfType<T>()
            .Subscribe(sink.OnNext);

        //

        private readonly Subject<LabeledValue> bus = new Subject<LabeledValue>();
    }
}