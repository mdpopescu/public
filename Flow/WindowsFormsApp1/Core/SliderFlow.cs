using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core
{
    public class SliderFlow : Flow
    {
        public SliderFlow(string key, string label, int startValue)
        {
            this.key = key;
            this.label = label;
            this.startValue = startValue;
        }

        //

        protected override IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs) =>
            inputs
                .SafeGet(key)
                .Transform<TrackBar>("ValueChanged", "value", trackBar => trackBar.Value);

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states) =>
            states
                .StartWith(new LabeledValue("value", startValue));

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs) =>
            new Dictionary<string, IObservable<LabeledValue>>
            {
                [key] = outputs.Transform<int>("value", "Text", value => $"{label}: {value}"),
                [key + "-values"] = outputs,
            };

        //

        private readonly string key;
        private readonly string label;
        private readonly int startValue;
    }
}