namespace ExpressionCompiler.Models
{
    public abstract class Token
    {
        public string Symbol { get; }

        //

        protected Token(string symbol)
        {
            Symbol = symbol;
        }
    }
}