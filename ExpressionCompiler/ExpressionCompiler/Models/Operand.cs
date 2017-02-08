namespace ExpressionCompiler.Models
{
    public abstract class Operand<T> : Token
    {
        public abstract T ActualValue { get; }

        protected Operand(string value)
            : base(value)
        {
        }
    }
}