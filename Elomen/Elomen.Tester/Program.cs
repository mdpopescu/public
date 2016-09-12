using System;
using Elomen.Library.Implementations;
using Elomen.Tester.Implementations;

namespace Elomen.Tester
{
    internal class Program
    {
        private static void Main()
        {
            var interpreter = new Interpreter(new NullAccountRepository(), new FakeCommandParser());

            do
            {
                Console.Write("Command: ");
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;

                var result = interpreter.Execute("", line);
                Console.WriteLine(result);
            } while (true);
        }
    }
}