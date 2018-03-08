using System;
using EventSystem.Library.Implementations;
using Tester.Models;

namespace Tester.Implementations
{
    public class NotificationGenerator
    {
        public NotificationGenerator()
        {
            EventBus.AddListener(_ => GenerateNotification(), it => it is Tick);
        }

        //

        private readonly Random rng = new Random();

        private void GenerateNotification()
        {
            var notification = new Notification(rng.Next(22), rng.Next(70), "*")
            {
                Background = (ConsoleColor) rng.Next(16),
                Foreground = (ConsoleColor) rng.Next(16),
            };
            EventBus.Publish(notification);
        }
    }
}