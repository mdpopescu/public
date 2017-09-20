using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WindowsFormsApp1.Attributes;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    [DeclareInput("weight-values", "value", "weight")]
    [DeclareInput("height-values", "value", "height")]
    public class BmiFlow : Flow
    {
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