using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp3.Contracts;
using WindowsFormsApp3.Extensions;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Core
{
    public class SliderFlow : FlowComponent
    {
        public SliderFlow(string tbLabel, string valueLabel, string textLabel, string textFormat, int initialValue)
        {
            this.tbLabel = tbLabel;
            this.valueLabel = valueLabel;
            this.textLabel = textLabel;
            this.textFormat = textFormat;
            this.initialValue = initialValue;
        }

        public IObservable<LabeledValue> Process(IObservable<LabeledValue> inputs)
        {
            // inputs

            var tbChanges = inputs
                .Extract<EventPattern<object>, TrackBar>(tbLabel, it => it.Sender)
                .Select(tb => tb.Value)
                .StartWith(initialValue);

            // computations

            var text = tbChanges
                .Select(value => string.Format(textFormat, value));

            // outputs

            var values = tbChanges
                .Select(value => new LabeledValue(valueLabel, value));

            var labels = text
                .Select(it => new LabeledValue(textLabel, it));

            // ReSharper disable once InvokeAsExtensionMethod
            return Observable.Merge(
                values,
                labels);
        }

        //

        private readonly string tbLabel;
        private readonly string valueLabel;
        private readonly string textLabel;
        private readonly string textFormat;
        private readonly int initialValue;
    }
}