using System;
using ExpressionCompiler.Contracts;

namespace ExpressionCompiler.Models
{
    public abstract class Operator<T> : Token, ICloneable, IBoostable
    {
        public int Priority { get; protected set; }
        public Func<T, T, T> Compute { get; }

        public abstract object Clone();

        public Token Boost(int boost)
        {
            var newOperator = (Operator<T>) Clone();
            newOperator.Priority += boost;

            return newOperator;
        }

        //

        protected Operator(string value, int priority, Func<T, T, T> compute)
            : base(value)
        {
            Priority = priority;
            Compute = compute;
        }
    }
}