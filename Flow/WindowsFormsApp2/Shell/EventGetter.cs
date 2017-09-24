using System;
using System.Reactive.Linq;
using System.Windows.Forms;
using WindowsFormsApp2.Models;

namespace WindowsFormsApp2.Shell
{
    public class EventGetter
    {
        public IObservable<LabeledValue> Get(Control control) =>
            control
                .GetAllControls()
                .Select(Extensions.GetAllEvents)
                .Merge();
    }
}