using System;
using Zaruri.Contracts;

namespace Zaruri.Services
{
    public class ConsoleReader : IReader
    {
        public string ReadLine() => Console.ReadLine();
    }
}