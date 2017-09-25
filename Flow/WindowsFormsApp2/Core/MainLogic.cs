using System;
using System.Reactive;
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
            var saves = inputs
                .Extract<Button, Unit>("btnSave", _ => Unit.Default);
            var restores = inputs
                .Extract<Button, Unit>("btnRestore", _ => Unit.Default);

            // ReSharper disable once InvokeAsExtensionMethod
            var bmis = Observable.CombineLatest(weights, heights, ComputeBMI);

            var savedWeights = weights.Whenever(saves);
            var savedHeights = heights.Whenever(saves);

            var lblWeight = weights
                .Select(w => $"Weight (kg): {w:##0}")
                .Select(text => new LabeledValue("lblWeight", "Text", text));
            var lblHeight = heights
                .Select(h => $"Height (cm): {h:##0}")
                .Select(text => new LabeledValue("lblHeight", "Text", text));
            var lblBMI = bmis
                .Select(bmi => $"BMI: {bmi:##0}")
                .Select(text => new LabeledValue("lblBMI", "Text", text));

            var lblSavedWeight = savedWeights
                .Select(w => $"Saved weight: {w:##0}")
                .Select(text => new LabeledValue("lblSavedWeight", "Text", text));
            var lblSavedHeight = savedHeights
                .Select(h => $"Saved height: {h:##0}")
                .Select(text => new LabeledValue("lblSavedHeight", "Text", text));

            var btnRestore = saves
                .Select(_ => true)
                .StartWith(false)
                .Select(value => new LabeledValue("btnRestore", "Enabled", value));

            var tbWeight = savedWeights
                .Whenever(restores)
                .Select(w => new LabeledValue("tbWeight", "Value", w));
            var tbHeight = savedHeights
                .Whenever(restores)
                .Select(h => new LabeledValue("tbHeight", "Value", h));

            return Observable.Merge(
                lblWeight,
                lblHeight,
                lblBMI,
                lblSavedWeight,
                lblSavedHeight,
                btnRestore,
                tbWeight,
                tbHeight);
        }

        private static int ComputeBMI(int w, int h) => (int) Math.Round(w / (h * h / 10000.0));
    }
}