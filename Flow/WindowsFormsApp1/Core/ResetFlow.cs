using System;
using System.Collections.Generic;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core
{
    public class ResetFlow : Flow
    {
        protected override IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs)
        {
            return inputs.SafeGet("reset");
        }

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states)
        {
            return states;
        }

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs)
        {
            return new Dictionary<string, IObservable<LabeledValue>>
            {
                ["set-weight"] = outputs.Transform<object>("Click", "Value", _ => 40),
                ["set-height"] = outputs.Transform<object>("Click", "Value", _ => 150),
            };
        }
    }
}