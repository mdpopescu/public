using System;
using EventSystem.Library.Implementations;
using Tester.Models;

namespace Tester.Implementations
{
    public class ScreenNotifier
    {
        public ScreenNotifier()
        {
            Console.Clear();
            Console.CursorVisible = false;

            EventBus.AddListener(DisplayNotification, it => it is Notification);
        }

        //

        private static void DisplayNotification(object obj)
        {
            var notification = (Notification) obj;
            Console.SetCursorPosition(notification.Col, notification.Row);
            Console.BackgroundColor = notification.Background;
            Console.ForegroundColor = notification.Foreground;
            Console.Write(notification.Text);
        }
    }
}