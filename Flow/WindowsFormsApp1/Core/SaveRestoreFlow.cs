using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core
{
    public class SaveRestoreFlow : Flow
    {
        protected override IObservable<LabeledValue> Intent(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs)
        {
            var weights = inputs
                .SafeGet("weight-values")
                .Relabel("value", "weight");
            var heights = inputs
                .SafeGet("height-values")
                .Relabel("value", "height");
            var saves = inputs
                .SafeGet("save")
                .Relabel("Click", "save");
            var restores = inputs
                .SafeGet("restore")
                .Relabel("Click", "restore");

            return Observable.Merge(weights, heights, saves, restores);
        }

        protected override IObservable<LabeledValue> Model(IObservable<LabeledValue> states)
        {
            var weights = states.Extract<int>("weight");
            var heights = states.Extract<int>("height");
            var saves = states.Extract<object>("save");
            var restores = states.Extract<object>("restore");

            var savedWeights = saves.CombineLatest(weights, (_, w) => w);
            var savedHeights = saves.CombineLatest(heights, (_, h) => h);

            var restoredWeights = restores.CombineLatest(savedWeights, (_, w) => new LabeledValue("restore-weight", w));
            var restoredHeights = restores.CombineLatest(savedHeights, (_, h) => new LabeledValue("restore-height", h));

            // ReSharper disable once InvokeAsExtensionMethod
            return Observable.Merge(
                savedWeights.Select(w => new LabeledValue("save-weight", w)),
                savedHeights.Select(h => new LabeledValue("save-height", h)),
                restoredWeights,
                restoredHeights);
        }

        protected override IReadOnlyDictionary<string, IObservable<LabeledValue>> View(IObservable<LabeledValue> outputs)
        {
            return new Dictionary<string, IObservable<LabeledValue>>
            {
                ["save-weight"] = outputs.Where(it => it.Label == "save-weight"),
                ["save-height"] = outputs.Where(it => it.Label == "save-height"),
                ["set-weight"] = outputs.Relabel("restore-weight", "Value"),
                ["set-height"] = outputs.Relabel("restore-height", "Value"),
            };
        }
    }
}