namespace ExpressionCompiler.Models
{
    public class Operation
    {
        public char Symbol { get; }
        public int Priority { get; }

        public Operation(char symbol, int priority)
        {
            Symbol = symbol;
            Priority = priority;
        }
    }
}