using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using WindowsFormsApp1.Core;
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
        public void SavesTheWeightAndHeightOnSave()
        {
            var results = sut.Process(inputs);

            var savedWeights = new List<int>();
            var savedHeights = new List<int>();

            results["save-weight"].Subscribe(it => savedWeights.Add((int) it.Value));
            results["save-height"].Subscribe(it => savedHeights.Add((int) it.Value));

            weights.OnNext(new LabeledValue("value", 12));
            heights.OnNext(new LabeledValue("value", 34));
            saves.OnNext(new LabeledValue("Click", null));

            CollectionAssert.AreEqual(new[] { 12 }, savedWeights);
            CollectionAssert.AreEqual(new[] { 34 }, savedHeights);
        }
    }
}