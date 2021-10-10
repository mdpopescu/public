using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Challenge2.Rx.Helpers
{
    public static class Extensions
    {
        public static IObservable<Unit> AsUnit<T>(this IObservable<T> source) =>
            source.Select(_ => Unit.Default);

        public static IObservable<Unit> GetClicks(this Control control) =>
            Observable.FromEventPattern(h => control.Click += h, h => control.Click -= h).AsUnit();

        public static IObservable<U> SelectSwitch<T, U>(this IObservable<T> source, Func<T, IObservable<U>> selector) =>
            source.Select(selector).Switch().Publish().RefCount();

        /// <summary>
        ///     Inverts the value of a boolean on each incoming event.
        /// </summary>
        /// <param name="source">The incoming events.</param>
        /// <param name="startValue">The starting value for the boolean flag.</param>
        /// <returns>A stream of T/F/T (or F/T/F) values with the same timing as the incoming events.</returns>
        public static IObservable<bool> Toggle<T>(this IObservable<T> source, bool startValue) =>
            source.Scan(startValue, (it, _) => !it).StartWith(startValue);

        public static void SetEnabled(this IObservable<bool> enabledChanges, Control control) =>
            enabledChanges.ObserveOn(control).Subscribe(SetEnabledFor(control));

        public static void SetText(this IObservable<string> textChanges, Control control) =>
            textChanges.ObserveOn(control).Subscribe(SetTextFor(control));

        //

        private static Action<bool> SetEnabledFor(Control control) => value => InternalSetEnabled(control, value);
        private static Action<string> SetTextFor(Control control) => value => InternalSetText(control, value);

        private static void InternalSetEnabled(Control control, bool value)
        {
            if (value)
            {
                control.Enabled = true;
                control.BackColor = Color.Lime;
            }
            else
            {
                control.Enabled = false;
                control.BackColor = SystemColors.Control;
            }
        }

        private static void InternalSetText(Control control, string text) => control.Text = text;
    }
}