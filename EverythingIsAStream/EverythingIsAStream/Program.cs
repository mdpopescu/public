using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace EverythingIsAStream
{
    internal class Program
    {
        private static void Main()
        {
            Take3();
        }

        private static void Take1()
        {
            var nameStream = Helpers.ToStream(() => ReadLine("Your name: "), Scheduler.CurrentThread);

            nameStream
                .TakeWhile(name => !string.IsNullOrWhiteSpace(name))
                .Subscribe(name => Console.WriteLine($"Hello, {name}"));
        }

        private static void Take2()
        {
            var nameStream = Helpers.ToStream(() => ReadLine("Your name: "), Scheduler.CurrentThread);
            var ageStream = Helpers.ToStream(() => int.Parse(ReadLine("Your age: ")), Scheduler.CurrentThread);

            var bothStream = nameStream.Zip(ageStream, (name, age) => new { name, age });

            bothStream
                .TakeWhile(both => !string.IsNullOrWhiteSpace(both.name) && both.age >= 18 && both.age < 100)
                .Subscribe(
                    both => Console.WriteLine($"Hello, {both.name} who is {both.age} years old!"),
                    ex => Console.WriteLine($"Error: {ex.Message}"));
        }

        private static void Take3()
        {
            var nameStream = Helpers
                .ToStream(() => ReadLine("Your name: "), Scheduler.CurrentThread)
                .TakeWhile(name => !string.IsNullOrWhiteSpace(name));
            var ageStream = Helpers
                .ToStream(() => int.Parse(ReadLine("Your age: ")), Scheduler.CurrentThread)
                .Where(age => age >= 18 && age < 100);

            var bothStream = from name in nameStream
                             from age in ageStream
                             select new { name, age };

            bothStream
                .Subscribe(
                    both => Console.WriteLine($"Hello, {both.name} who is {both.age} years old!"),
                    ex => Console.WriteLine($"Error: {ex.Message}"));
        }

        //

        private static string ReadLine(string prefix)
        {
            Console.Write(prefix);
            return Console.ReadLine();
        }
    }
}