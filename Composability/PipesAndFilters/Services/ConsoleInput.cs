using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class ConsoleInput : ISource<string>
    {
        public ConsoleInput(string prompt)
        {
            this.prompt = prompt ?? "?";
        }

        public string Process(Unit _)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        //

        private readonly string prompt;
    }
}