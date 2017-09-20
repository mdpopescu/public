using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    public class SaveRestoreFlow : Flow
    {
        protected override IEnumerable<InputSelection> DeclareInputs() =>
            new[]
            {
                new InputSelection("weight-values", "value", "weight"),
                new InputSelection("height-values", "value", "height"),
                new InputSelection("save", "Click", "save"),
                new InputSelection("restore", "Click", "restore"),
            };

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states)
        {
            var weights = states.Extract<int>("weight");
            var heights = states.Extract<int>("height");
            var saves = states.Extract<object>("save");
            var restores = states.Extract<object>("restore");

            // ReSharper disable once InvokeAsExtensionMethod
            var both = Observable.CombineLatest(weights, heights, Tuple.Create);

            var saved = both.Whenever(saves);
            var restored = saved.Whenever(restores);

            var restoredWeights = restored.Select(wh => new LabeledValue("restore-weight", wh.Item1));
            var restoredHeights = restored.Select(wh => new LabeledValue("restore-height", wh.Item2));

            var enableRestore = saves
                .Take(1)
                .Select(_ => new LabeledValue("enable-restore", true));

            // ReSharper disable once InvokeAsExtensionMethod
            return Observable.Merge(
                saved.Select(wh => new LabeledValue("save-weight", wh.Item1)),
                saved.Select(wh => new LabeledValue("save-height", wh.Item2)),
                restoredWeights,
                restoredHeights,
                enableRestore);
        }

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs)
        {
            return new Dictionary<string, IObservable<LabeledValue>>
            {
                ["save-weight"] = outputs.Transform<int>("save-weight", "Text", w => $"Saved weight: {w}"),
                ["save-height"] = outputs.Transform<int>("save-height", "Text", h => $"Saved height: {h}"),
                ["set-weight"] = outputs.Relabel("restore-weight", "Value"),
                ["set-height"] = outputs.Relabel("restore-height", "Value"),
                ["enable-restore"] = outputs.Relabel("enable-restore", "Enabled"),
            };
        }
    }
}