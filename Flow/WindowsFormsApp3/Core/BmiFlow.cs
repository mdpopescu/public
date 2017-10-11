using System;
using System.Reactive.Linq;
using WindowsFormsApp3.Contracts;
using WindowsFormsApp3.Extensions;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Core
{
    public class BmiFlow : FlowComponent
    {
        public IObservable<LabeledValue> Process(IObservable<LabeledValue> inputs)
        {
            // inputs

            var weights = inputs
                .Extract<int>("weights");
            var heights = inputs
                .Extract<int>("heights");

            // computations

            var bmi = weights
                .CombineLatest(heights, (w, h) => (int) Math.Round(w / (h * h / 10000.0)));
            var text = bmi
                .Select(value => $"BMI: {value:##0}");

            // outputs

            var labels = text
                .Select(it => new LabeledValue("lblBMI:Text", it));

            return labels;
        }
    }
}