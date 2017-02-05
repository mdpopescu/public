using System;

namespace ExpressionCompiler.Models
{
    public class Operation
    {
        public char Symbol { get; }
        public int Priority { get; }
        public Func<int, int, int> Compute { get; }

        public Operation(char symbol, int priority, Func<int, int, int> compute)
        {
            Symbol = symbol;
            Priority = priority;
            Compute = compute;
        }
    }
}