using System;
using System.IO;
using PipesAndFilters.Contracts;
using PipesAndFilters.Models;

namespace PipesAndFilters.Services
{
    public class SessionGenerator : IFilter<bool, IEffect>
    {
        public SessionGenerator(TextWriter successWriter, TextWriter errorWriter)
        {
            this.successWriter = successWriter;
            this.errorWriter = errorWriter;
        }

        public IEffect Process(bool input)
        {
            return input
                ? new TextOutput(successWriter, Guid.NewGuid().ToString("N"))
                : new TextOutput(errorWriter, "Invalid session.");
        }

        //

        private readonly TextWriter successWriter;
        private readonly TextWriter errorWriter;
    }
}