using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    public class BmiFlow : Flow
    {
        protected override IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs)
        {
            var weights = inputs
                .SafeGet("weight-values")
                .Relabel("value", "weight");
            var heights = inputs
                .SafeGet("height-values")
                .Relabel("value", "height");

            return weights.Merge(heights);
        }

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states)
        {
            var weights = states.Extract<int>("weight");
            var heights = states.Extract<int>("height");

            return weights
                .CombineLatest(heights, (w, h) => (int) Math.Round(w / (h * h / 10000.0)))
                .Select(bmi => new LabeledValue("bmi", bmi));
        }

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs) =>
            new Dictionary<string, IObservable<LabeledValue>>
            {
                ["bmi"] = outputs.Transform<int>("bmi", "Text", bmi => $"BMI: {bmi}"),
            };
    }
}