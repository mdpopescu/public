using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DesktopClock
{
    public static class ControlExtensions
    {
        public static void UIChange<T>(this Control control, Action<T> action, T data)
        {
            control.Dispatcher.Invoke(DispatcherPriority.Normal, action, data);
        }
    }
}