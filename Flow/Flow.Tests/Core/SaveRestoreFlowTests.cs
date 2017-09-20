using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using WindowsFormsApp1.Core;
using WindowsFormsApp1.Core.Flows;
using WindowsFormsApp1.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flow.Tests.Core
{
    [TestClass]
    public class SaveRestoreFlowTests
    {
        private Subject<LabeledValue> weights;
        private Subject<LabeledValue> heights;
        private Subject<LabeledValue> saves;
        private Subject<LabeledValue> restores;

        private Dictionary<string, IObservable<LabeledValue>> inputs;

        private SaveRestoreFlow sut;

        [TestInitialize]
        public void SetUp()
        {
            weights = new Subject<LabeledValue>();
            heights = new Subject<LabeledValue>();
            saves = new Subject<LabeledValue>();
            restores = new Subject<LabeledValue>();

            inputs = new Dictionary<string, IObservable<LabeledValue>>
            {
                ["weight-values"] = weights,
                ["height-values"] = heights,
                ["save"] = saves,
                ["restore"] = restores,
            };

            sut = new SaveRestoreFlow();
        }

        [TestMethod]
        public void SavesTheWeightAndHeight()
        {
            var results = sut.Process(inputs);

            var savedWeights = new List<string>();
            var savedHeights = new List<string>();

            results["save-weight"].Subscribe(it => savedWeights.Add((string) it.Value));
            results["save-height"].Subscribe(it => savedHeights.Add((string) it.Value));

            weights.OnNext(new LabeledValue("value", 12));
            heights.OnNext(new LabeledValue("value", 34));
            saves.OnNext(new LabeledValue("Click", null));

            CollectionAssert.AreEqual(new[] { "Saved weight: 12" }, savedWeights);
            CollectionAssert.AreEqual(new[] { "Saved height: 34" }, savedHeights);
        }

        [TestMethod]
        public void RestoresTheWeightAndHeight()
        {
            var results = sut.Process(inputs);

            var setWeights = new List<int>();
            var setHeights = new List<int>();

            results["set-weight"].Subscribe(it => setWeights.Add((int) it.Value));
            results["set-height"].Subscribe(it => setHeights.Add((int) it.Value));

            weights.OnNext(new LabeledValue("value", 12));
            heights.OnNext(new LabeledValue("value", 34));
            saves.OnNext(new LabeledValue("Click", null));
            restores.OnNext(new LabeledValue("Click", null));

            CollectionAssert.AreEqual(new[] { 12 }, setWeights);
            CollectionAssert.AreEqual(new[] { 34 }, setHeights);
        }
    }
}