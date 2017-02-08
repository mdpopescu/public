namespace ExpressionCompiler.Models.Operators
{
    public class IntegerDivision : IntegerOperator
    {
        public IntegerDivision()
            : base("/", 2, (a, b) => a / b)
        {
        }

        public override object Clone() => new IntegerDivision();
    }
}