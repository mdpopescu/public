using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.Models;

namespace WindowsFormsApp2.Core
{
    public class MainLogic
    {
        public IObservable<LabeledValue> ViewChanges { get; }

        public MainLogic(IObservable<LabeledValue> inputs)
        {
            ViewChanges = Process(inputs);
        }

        //

        private static IObservable<LabeledValue> Process(IObservable<LabeledValue> inputs)
        {
            var weights = inputs
                .Extract<TrackBar, int>("tbWeight", it => it.Value)
                .StartWith(40);
            var heights = inputs
                .Extract<TrackBar, int>("tbHeight", it => it.Value)
                .StartWith(150);

            // ReSharper disable once InvokeAsExtensionMethod
            var bmis = Observable.CombineLatest(weights, heights, ComputeBMI);

            var lblWeight = weights
                .Select(w => $"Weight (kg): {w:##0}")
                .Select(text => new LabeledValue("lblWeight", "Text", text));
            var lblHeight = heights
                .Select(h => $"Height (cm): {h:##0}")
                .Select(text => new LabeledValue("lblHeight", "Text", text));
            var lblBMI = bmis
                .Select(bmi => $"BMI: {bmi:##0}")
                .Select(text => new LabeledValue("lblBMI", "Text", text));

            return Observable.Merge(
                lblWeight,
                lblHeight,
                lblBMI);
        }

        private static int ComputeBMI(int w, int h) => (int) Math.Round(w / (h * h / 10000.0));
    }
}