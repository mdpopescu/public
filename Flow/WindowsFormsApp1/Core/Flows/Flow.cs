using System;
using System.Collections.Generic;
using WindowsFormsApp1.Contracts;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    public abstract class Flow : IFlow
    {
        public virtual IReadOnlyDictionary<string, IObservable<LabeledValue>> Process(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs)
        {
            var states = Intent(inputs);
            var outputs = Model(states);
            var updates = View(outputs);

            return updates;
        }

        //

        protected abstract IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs);
        protected abstract IObservable<LabeledValue> Model(IObservable<LabeledValue> states);
        protected abstract IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs);
    }
}