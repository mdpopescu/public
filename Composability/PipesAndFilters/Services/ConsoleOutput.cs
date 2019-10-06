using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class ConsoleOutput : ITarget<string>
    {
        public ConsoleOutput(string format)
        {
            this.format = format ?? "{0}";
        }

        public Unit Process(string input)
        {
            Console.WriteLine(format, input);
            return Unit.INSTANCE;
        }

        //

        private readonly string format;
    }
}