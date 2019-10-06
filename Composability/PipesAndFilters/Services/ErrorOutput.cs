using System;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class ErrorOutput : ITarget<Exception>
    {
        public ErrorOutput(string format)
        {
            this.format = format ?? "?";
        }

        public Unit Process(Exception ex)
        {
            Console.Error.WriteLine(format, ex.Message);
            return Unit.INSTANCE;
        }

        //

        private readonly string format;
    }
}