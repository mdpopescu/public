using System;
using System.Reactive.Linq;
using EventSystem.Library.Implementations;
using Tester.Models;

namespace Tester
{
    internal class Program
    {
        private static readonly Random RNG = new Random();

        private static void Main()
        {
            Console.Clear();
            Console.CursorVisible = false;

            Observable.Interval(TimeSpan.FromMilliseconds(5)).Subscribe(_ => EventBus.Publish(new Tick()));

            EventBus.Get<Tick>().Subscribe(_ => GenerateNotification());
            EventBus.Get<Notification>().Subscribe(DisplayNotification);

            Console.ReadLine();

            Console.CursorVisible = true;
        }

        private static void GenerateNotification()
        {
            var notification = new Notification(RNG.Next(22), RNG.Next(70), "*")
            {
                Background = (ConsoleColor) RNG.Next(16),
                Foreground = (ConsoleColor) RNG.Next(16),
            };
            EventBus.Publish(notification);
        }

        private static void DisplayNotification(Notification notification)
        {
            Console.SetCursorPosition(notification.Col, notification.Row);
            Console.BackgroundColor = notification.Background;
            Console.ForegroundColor = notification.Foreground;
            Console.Write(notification.Text);
        }
    }
}