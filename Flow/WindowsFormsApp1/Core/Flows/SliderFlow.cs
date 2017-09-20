using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    public class SliderFlow : Flow
    {
        public SliderFlow(string category, string label, int startValue)
        {
            this.category = category;
            this.label = label;
            this.startValue = startValue;
        }

        //

        protected override IEnumerable<InputSelection> DeclareInputs() => new[] { new InputSelection(category) };

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states) =>
            states
                .Transform<TrackBar>("ValueChanged", "value", trackBar => trackBar.Value)
                .StartWith(new LabeledValue("value", startValue));

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs) =>
            new Dictionary<string, IObservable<LabeledValue>>
            {
                [category] = outputs.Transform<int>("value", "Text", value => $"{label}: {value}"),
                [category + "-values"] = outputs,
            };

        //

        private readonly string category;
        private readonly string label;
        private readonly int startValue;
    }
}