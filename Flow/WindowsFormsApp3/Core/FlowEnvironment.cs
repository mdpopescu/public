using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using WindowsFormsApp3.Contracts;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Core
{
    public class FlowEnvironment
    {
        public void AddInput<T>(string label, IObservable<T> source) => sources
            .Add(Tuple.Create(label, source.Select(it => (object) it)));

        public void AddComponent(FlowComponent component) => components
            .Add(component);

        public void AddOutput<T>(string label, Action<T> sink) => sinks
            .Add(Tuple.Create<string, Type, Action<object>>(label, typeof(T), value => sink((T) value)));

        public void Start()
        {
            foreach (var source in sources)
                source
                    .Item2
                    .Select(value => new LabeledValue(source.Item1, value))
                    .Subscribe(bus.OnNext);

            foreach (var sink in sinks)
                bus
                    .Where(lv => string.Equals(sink.Item1, lv.Label, StringComparison.OrdinalIgnoreCase))
                    .Select(lv => lv.Value)
                    .Where(it => it != null && it.GetType() == sink.Item2)
                    .Subscribe(sink.Item3);

            foreach (var component in components)
                component
                    .Process(bus)
                    .Subscribe(bus.OnNext);
        }

        //

        private readonly Subject<LabeledValue> bus = new Subject<LabeledValue>();

        private readonly List<Tuple<string, IObservable<object>>> sources = new List<Tuple<string, IObservable<object>>>();
        private readonly List<FlowComponent> components = new List<FlowComponent>();
        private readonly List<Tuple<string, Type, Action<object>>> sinks = new List<Tuple<string, Type, Action<object>>>();
    }
}