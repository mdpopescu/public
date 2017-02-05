using System;

namespace ExpressionCompiler.Models
{
    public class OperationEx : Operation
    {
        public int Index { get; }

        public OperationEx(int index, char symbol, int priority, Func<int, int, int> compute)
            : base(symbol, priority, compute)
        {
            Index = index;
        }
    }
}