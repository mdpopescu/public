using System;

namespace ExpressionCompiler.Models
{
    public class Operator<T> : Token
    {
        public Operator(string value, int intrinsicPriority, Func<T, T, T> compute)
            : base(value)
        {
            IntrinsicPriority = intrinsicPriority;
            Compute = compute;
        }

        public int IntrinsicPriority { get; }
        public Func<T, T, T> Compute { get; }
    }
}