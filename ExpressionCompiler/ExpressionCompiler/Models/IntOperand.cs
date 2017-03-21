namespace ExpressionCompiler.Models
{
    public class IntOperand : Operand<int>
    {
        public IntOperand(string symbol) : base(symbol)
        {
            Value = int.Parse(symbol);
        }
    }
}