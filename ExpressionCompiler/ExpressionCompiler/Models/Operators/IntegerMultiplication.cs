namespace ExpressionCompiler.Models.Operators
{
    public class IntegerMultiplication : Operator<int>
    {
        public IntegerMultiplication()
            : base("*", 2, (a, b) => a * b)
        {
        }

        public override object Clone() => new IntegerMultiplication();
    }
}