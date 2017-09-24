using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.Models;

namespace WindowsFormsApp2
{
    public static class Extensions
    {
        /// <summary>Executes an action if the given object is not <c>null</c>.</summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="action">The action.</param>
        public static void IfNotNull<T>(this T obj, Action<T> action)
        {
            if (obj != null)
                action(obj);
        }

        public static IObservable<Control> GetAllControls(this Control control)
        {
            var controls = control.Controls.Cast<Control>().ToObservable();
            return controls.Concat(controls.SelectMany(GetAllControls));
        }

        public static IObservable<LabeledValue> GetAllEvents(this Control control) =>
            control.Intercept("Click", "TextChanged", "ValueChanged");

        public static IObservable<TR> Extract<T, TR>(this IObservable<LabeledValue> inputs, string id, Func<T, TR> selector) =>
            inputs
                .Where(it => string.Equals(it.Id, id, StringComparison.OrdinalIgnoreCase))
                .Select(it => it.Value)
                .OfType<T>()
                .Select(selector);

        //

        private static IObservable<LabeledValue> Intercept(this Control control, params string[] events) =>
            events
                .Select(eventName => InterceptSingle(control, eventName))
                .Merge();

        private static IObservable<LabeledValue> InterceptSingle(Control control, string eventName)
        {
            try
            {
                return Observable
                    .FromEventPattern(control, eventName)
                    .Select(it => new LabeledValue(control.Name, eventName, control));
            }
            catch
            {
                return Observable.Never<LabeledValue>();
            }
        }
    }
}