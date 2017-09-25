using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using WindowsFormsApp2.Core;
using WindowsFormsApp2.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flow.Tests.Core
{
    [TestClass]
    public class MainLogicTests
    {
        private MainLogic sut;

        private Subject<LabeledValue> inputs;
        private List<LabeledValue> outputs;

        [TestInitialize]
        public void SetUp()
        {
            sut = new MainLogic();

            inputs = new Subject<LabeledValue>();
            outputs = new List<LabeledValue>();

            sut.Process(inputs).Subscribe(outputs.Add);
        }

        [TestMethod]
        public void SetsInitialValues()
        {
            // I don't care about the order
            CollectionAssert.AreEquivalent(
                new[]
                {
                    "lblWeight:Text = [Weight (kg):  40]",
                    "lblHeight:Text = [Height (cm): 150]",
                    "lblBMI:Text = [BMI:  18]",
                    "btnRestore:Enabled = [False]",
                },
                outputs.Select(it => it.ToString()).ToArray());
        }

        [TestMethod]
        public void UpdatesTheWeightLabelTextWhenTheTrackBarChanges()
        {
            inputs.OnNext(new LabeledValue("tbWeight", "Value", new TrackBar { Minimum = 40, Maximum = 150, Value = 80 }));

            Assert.AreEqual("lblWeight", outputs[4].Id);
            Assert.AreEqual("Text", outputs[4].Label);
            Assert.AreEqual("Weight (kg):  80", outputs[4].Value);
        }

        [TestMethod]
        public void UpdatesTheHeightLabelTextWhenTheTrackBarChanges()
        {
            inputs.OnNext(new LabeledValue("tbHeight", "Value", new TrackBar { Minimum = 150, Maximum = 220, Value = 180 }));

            Assert.AreEqual("lblHeight", outputs[4].Id);
            Assert.AreEqual("Text", outputs[4].Label);
            Assert.AreEqual("Height (cm): 180", outputs[4].Value);
        }

        [TestMethod]
        public void UpdatesTheBMILabelTextWhenTheWeightTrackBarChanges()
        {
            inputs.OnNext(new LabeledValue("tbWeight", "Value", new TrackBar { Minimum = 40, Maximum = 150, Value = 50 }));

            Assert.AreEqual("lblBMI", outputs[5].Id);
            Assert.AreEqual("Text", outputs[5].Label);
            Assert.AreEqual("BMI:  22", outputs[5].Value);
        }

        [TestMethod]
        public void UpdatesTheBMILabelTextWhenTheHeightTrackBarChanges()
        {
            inputs.OnNext(new LabeledValue("tbHeight", "Value", new TrackBar { Minimum = 150, Maximum = 220, Value = 180 }));

            Assert.AreEqual("lblBMI", outputs[5].Id);
            Assert.AreEqual("Text", outputs[5].Label);
            Assert.AreEqual("BMI:  12", outputs[5].Value);
        }
    }
}