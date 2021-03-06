﻿namespace WindowsFormsApp1.Models
{
    public class LabeledValue
    {
        public string Label { get; }
        public object Value { get; }

        public LabeledValue(string label, object value)
        {
            Label = label;
            Value = value;
        }

        public override string ToString() => $"{Label}: {Value}";
    }
}