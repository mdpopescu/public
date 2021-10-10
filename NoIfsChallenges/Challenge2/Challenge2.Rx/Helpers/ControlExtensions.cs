using System;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Challenge2.Rx.Helpers
{
    public static class ControlExtensions
    {
        public static IObservable<Unit> GetClicks(this Control control) =>
            Observable.FromEventPattern(h => control.Click += h, h => control.Click -= h).AsUnit();

        public static void SetEnabled(this Control control, IObservable<bool> enabledChanges) =>
            enabledChanges.ObserveOn(control).Subscribe(SetEnabledFor(control));

        public static void SetText(this Control control, IObservable<string> textChanges) =>
            textChanges.ObserveOn(control).Subscribe(SetTextFor(control));

        public static Action<bool> SetEnabledFor(this Control control) => value => InternalSetEnabled(control, value);
        public static Action<string> SetTextFor(this Control control) => value => InternalSetText(control, value);

        public static void InternalSetEnabled(this Control control, bool value)
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

        public static void InternalSetText(this Control control, string text) => control.Text = text;
    }
}