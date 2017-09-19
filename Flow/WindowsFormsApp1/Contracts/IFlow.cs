using System;
using System.Collections.Generic;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Contracts
{
    public interface IFlow
    {
        IReadOnlyDictionary<string, IObservable<LabeledValue>> Process(IReadOnlyDictionary<string, IObservable<LabeledValue>> inputs);
    }
}