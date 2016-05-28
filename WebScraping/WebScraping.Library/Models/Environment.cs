using System;
using System.IO;

namespace WebScraping.Library.Models
{
    public class Environment : IDisposable
    {
        public TextReader Input { get; }
        public TextWriter Output { get; }

        public Environment(TextReader input, TextWriter output)
        {
            Input = input;
            Output = output;
        }

        public void Dispose()
        {
            Output.Dispose();
            Input.Dispose();
        }
    }
}