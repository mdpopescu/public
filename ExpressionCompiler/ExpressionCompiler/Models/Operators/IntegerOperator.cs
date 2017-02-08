using System;

namespace ExpressionCompiler.Models.Operators
{
    public abstract class IntegerOperator : Operator<int>
    {
        protected IntegerOperator(string value, int priority, Func<int, int, int> compute)
            : base(value, priority, compute)
        {
        }
    }
}