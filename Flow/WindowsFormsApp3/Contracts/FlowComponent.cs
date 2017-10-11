using System;
using WindowsFormsApp3.Models;

namespace WindowsFormsApp3.Contracts
{
    public interface FlowComponent
    {
        IObservable<LabeledValue> Process(IObservable<LabeledValue> inputs);
    }
}