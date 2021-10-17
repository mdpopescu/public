using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Challenge2.Rx.Helpers
{
    public static class ControlExtensions
    {
        public static IObservable<Unit> GetClicks(this Control control) =>
            Observable.FromEventPattern(h => control.Click += h, h => control.Click -= h).AsUnit();

        public static void HandleChanges<T>(this Control control, IObservable<T> changes, Action<Control, T> handler) =>
            changes.ObserveOn(control).Subscribe(value => handler(control, value));
    }
}