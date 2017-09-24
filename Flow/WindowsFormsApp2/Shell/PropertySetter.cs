using System;
using System.Reflection;
using WindowsFormsApp2.Contracts;
using WindowsFormsApp2.Models;

namespace WindowsFormsApp2.Shell
{
    public class PropertySetter : IOutput
    {
        public PropertySetter(Func<string, object> locator)
        {
            this.locator = locator;
        }

        public void Set(LabeledValue labeledValue)
        {
            locator(labeledValue.Id)
                .IfNotNull(
                    target => target
                        .GetType()
                        .GetProperty(labeledValue.Label, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                        .IfNotNull(p => p.SetValue(target, labeledValue.Value)));
        }

        //

        private readonly Func<string, object> locator;
    }
}