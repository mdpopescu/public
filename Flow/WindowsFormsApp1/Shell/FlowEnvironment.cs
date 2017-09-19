﻿using System;
using System.Collections.Generic;
using WindowsFormsApp1.Contracts;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Shell
{
    public class FlowEnvironment : IDisposable
    {
        public void Dispose()
        {
            foreach (var subscription in subscriptions)
                subscription.Dispose();
        }

        public void AddInput(string key, IObservable<LabeledValue> inputs)
        {
            var inputEnvironment = new Dictionary<string, IObservable<LabeledValue>> { [key] = inputs };
            environment = Extensions.Combine(environment, inputEnvironment);
        }

        public void AddFlow(IFlow flow)
        {
            environment = Extensions.Combine(environment, flow.Process(environment));
        }

        public void AddOutput(string key, IOutput output)
        {
            subscriptions.Add(environment.SafeGet(key).Subscribe(output.Set));
        }

        //

        private readonly List<IDisposable> subscriptions = new List<IDisposable>();

        private IReadOnlyDictionary<string, IObservable<LabeledValue>> environment = new Dictionary<string, IObservable<LabeledValue>>();
    }
}