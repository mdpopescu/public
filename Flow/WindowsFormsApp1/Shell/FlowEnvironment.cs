using System;
using System.Collections.Generic;
using WindowsFormsApp1.Core;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Shell
{
    public class FlowEnvironment
    {
        public void AddInput(string key, IObservable<LabeledValue> inputs)
        {
            var inputEnvironment = new Dictionary<string, IObservable<LabeledValue>> { [key] = inputs };
            environment = Extensions.Combine(environment, inputEnvironment);
        }

        public void AddFlow(Flow flow)
        {
            environment = Extensions.Combine(environment, flow.Process(environment));
        }

        public IDisposable AddOutput(string key, object output)
        {
            return environment.AddOutput(key, output);
        }

        //

        private IReadOnlyDictionary<string, IObservable<LabeledValue>> environment = new Dictionary<string, IObservable<LabeledValue>>();
    }
}