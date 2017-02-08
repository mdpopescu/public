namespace ExpressionCompiler.Models.Operators
{
    public class IntegerAddition : IntegerOperator
    {
        public IntegerAddition()
            : base("+", 1, (a, b) => a + b)
        {
        }

        public override object Clone() => new IntegerAddition();
    }
}