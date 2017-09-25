using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.Models;

namespace WindowsFormsApp2.Core
{
    public class MainLogic
    {
        public IObservable<LabeledValue> Process(IObservable<LabeledValue> inputs)
        {
            // extract inputs

            var weights = inputs
                .Extract<TrackBar, int>("tbWeight", it => it.Value)
                .StartWith(40);
            var heights = inputs
                .Extract<TrackBar, int>("tbHeight", it => it.Value)
                .StartWith(150);
            var saves = inputs
                .Extract<Button>("btnSave");
            var restores = inputs
                .Extract<Button>("btnRestore");

            // perform computations

            var bmis = weights.CombineLatest(heights, ComputeBMI);

            var savedWeights = weights.Whenever(saves);
            var savedHeights = heights.Whenever(saves);

            // generate outputs

            var lblWeight = weights
                .Select(w => $"Weight (kg): {w,3:##0}")
                .Select(text => new LabeledValue("lblWeight", "Text", text));
            var lblHeight = heights
                .Select(h => $"Height (cm): {h,3:##0}")
                .Select(text => new LabeledValue("lblHeight", "Text", text));
            var lblBMI = bmis
                .Select(bmi => $"BMI: {bmi,3:##0}")
                .Select(text => new LabeledValue("lblBMI", "Text", text));

            var lblSavedWeight = savedWeights
                .Select(w => $"Saved weight: {w,3:##0}")
                .Select(text => new LabeledValue("lblSavedWeight", "Text", text));
            var lblSavedHeight = savedHeights
                .Select(h => $"Saved height: {h,3:##0}")
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

            // return the merged outputs

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

        //

        private static int ComputeBMI(int w, int h) => (int) Math.Round(w / (h * h / 10000.0));
    }
}