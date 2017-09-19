using System.Reflection;
using WindowsFormsApp1.Contracts;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Core
{
    public class PropertySetter : IOutput
    {
        public PropertySetter(object target)
        {
            this.target = target;
        }

        public void Set(LabeledValue labeledValue)
        {
            target
                .GetType()
                .GetProperty(labeledValue.Label, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                .Do(p => p.SetValue(target, labeledValue.Value));
        }

        //

        private readonly object target;
    }
}