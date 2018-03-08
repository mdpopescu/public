using System;
using Tester.Implementations;

namespace Tester
{
    internal class Program
    {
        private static void Main()
        {
            // ReSharper disable ObjectCreationAsStatement
            new ScreenNotifier();
            new NotificationGenerator();
            new TickGenerator();
            // ReSharper restore ObjectCreationAsStatement

            Console.ReadLine();
        }
    }
}