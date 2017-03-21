namespace ExpressionCompiler.Models
{
    public abstract class Operand<T> : Token
    {
        public T Value { get; protected set; }

        //

        protected Operand(string symbol) : base(symbol)
        {
        }
    }
}