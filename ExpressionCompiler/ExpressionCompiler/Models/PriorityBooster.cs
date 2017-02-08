namespace ExpressionCompiler.Models
{
    public abstract class PriorityBooster : Token
    {
        protected PriorityBooster(string value)
            : base(value)
        {
        }

        public abstract int Boost { get; }
    }
}