namespace ExpressionCompiler.Models.Operators
{
    public class IntegerAddition : Operator<int>
    {
        public IntegerAddition()
            : base("+", 1, (a, b) => a + b)
        {
        }

        public override object Clone() => new IntegerAddition();
    }
}