using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace EverythingIsAStream
{
    internal class Program
    {
        private static void Main()
        {
            Take2();
        }

        private static void Take1()
        {
            SCHEDULER
                .Run(() => ReadLine("Your name: "))
                .TakeWhile(name => !string.IsNullOrWhiteSpace(name))
                .Subscribe(name => Console.WriteLine($"Hello, {name}"));
        }

        private static void Take2()
        {
            bool IsValid(string name, int age) => !string.IsNullOrWhiteSpace(name) && age >= 18 && age < 100;

            var nameStream = SCHEDULER.Run(() => ReadLine("Your name: "));
            var ageStream = SCHEDULER.Run(() => int.Parse(ReadLine("Your age: ")));

            var bothStream = nameStream.Zip(ageStream, (name, age) => (name, age));

            bothStream
                .TakeWhile(both => IsValid(both.name, both.age))
                .Subscribe(both => WriteSalutation(both.name, both.age), HandleError);
        }

        private static void Take3()
        {
            var nameStream = SCHEDULER
                .Run(() => ReadLine("Your name: "))
                .TakeWhile(name => !string.IsNullOrWhiteSpace(name));
            var ageStream = SCHEDULER
                .Run(() => int.Parse(ReadLine("Your age: ")))
                .TakeWhile(age => age >= 18 && age < 100);

            var bothStream = from name in nameStream
                             from age in ageStream
                             select new { name, age };

            bothStream.Subscribe(both => WriteSalutation(both.name, both.age), HandleError);
        }

        //

        private static readonly IScheduler SCHEDULER = Scheduler.CurrentThread;

        private static string ReadLine(string prefix)
        {
            Console.Write(prefix);
            return Console.ReadLine();
        }

        private static void WriteSalutation(string name, int age) =>
            Console.WriteLine($"Hello, {name} who is {age} years old!");

        private static void HandleError(Exception ex) =>
            Console.WriteLine($"Error: {ex.Message}");
    }
}