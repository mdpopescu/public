﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using WindowsFormsApp1.Attributes;
using WindowsFormsApp1.Contracts;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    public abstract class Flow : IFlow
    {
        public virtual IReadOnlyDictionary<string, IObservable<LabeledValue>> Link(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs)
        {
            var states = Intent(inputs);
            var outputs = Model(states);
            var updates = View(outputs);

            return updates;
        }

        //

        protected virtual DeclareInputAttribute[] DeclaredInputs { get; }

        protected Flow()
        {
            DeclaredInputs = Attribute
                .GetCustomAttributes(GetType(), typeof(DeclareInputAttribute))
                .Cast<DeclareInputAttribute>()
                .ToArray();
        }

        protected virtual IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs) =>
            DeclaredInputs
                .ToObservable()
                .Select(inputs.SelectInput)
                .Merge();

        protected abstract IObservable<LabeledValue> Model(IObservable<LabeledValue> states);
        protected abstract IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs);
    }
}