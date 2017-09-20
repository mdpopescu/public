using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WindowsFormsApp1.Attributes;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core.Flows
{
    [DeclareInput("weight-values", "value", "weight")]
    [DeclareInput("height-values", "value", "height")]
    [DeclareInput("save", "Click", "save")]
    [DeclareInput("restore", "Click", "restore")]
    public class SaveRestoreFlow : Flow
    {
        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states)
        {
            var weights = states.Extract<int>("weight");
            var heights = states.Extract<int>("height");
            var saves = states.Extract<object>("save");
            var restores = states.Extract<object>("restore");

            var both = weights.CombineLatest(heights, Tuple.Create);

            var saved = both.Whenever(saves);
            var savedWeights = saved.Select(wh => new LabeledValue("save-weight", wh.Item1));
            var savedHeights = saved.Select(wh => new LabeledValue("save-height", wh.Item2));

            var restored = saved.Whenever(restores);
            var restoredWeights = restored.Select(wh => new LabeledValue("restore-weight", wh.Item1));
            var restoredHeights = restored.Select(wh => new LabeledValue("restore-height", wh.Item2));

            var enableRestore = saves
                .Take(1)
                .Select(_ => new LabeledValue("enable-restore", true));

            // ReSharper disable once InvokeAsExtensionMethod
            return Observable.Merge(
                savedWeights,
                savedHeights,
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