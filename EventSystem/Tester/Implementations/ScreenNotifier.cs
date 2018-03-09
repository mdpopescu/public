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

            EventBus.Get<Notification>().Subscribe(DisplayNotification);
        }

        //

        private static void DisplayNotification(Notification notification)
        {
            Console.SetCursorPosition(notification.Col, notification.Row);
            Console.BackgroundColor = notification.Background;
            Console.ForegroundColor = notification.Foreground;
            Console.Write(notification.Text);
        }
    }
}