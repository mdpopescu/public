namespace ExpressionCompiler.Models
{
    public abstract class Operator : Token
    {
        protected Operator(string symbol) : base(symbol)
        {
        }
    }
}