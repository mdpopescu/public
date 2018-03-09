using System;
using System.Reactive.Linq;
using EventSystem.Library.Implementations;
using EventSystem.Library.Models;
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

            EventBus.AddSource(Observable.Interval(TimeSpan.FromMilliseconds(5)).Select(_ => new LabeledValue("tick", null)));
            EventBus.AddTransformation("tick", _ => new LabeledValue("notification", GenerateNotification()));
            EventBus.AddSink("notification", it => DisplayNotification((Notification) it.Value));

            Console.ReadLine();

            Console.CursorVisible = true;
        }

        private static Notification GenerateNotification()
        {
            return new Notification(RNG.Next(22), RNG.Next(70), "*")
            {
                Background = (ConsoleColor) RNG.Next(16),
                Foreground = (ConsoleColor) RNG.Next(16),
            };
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