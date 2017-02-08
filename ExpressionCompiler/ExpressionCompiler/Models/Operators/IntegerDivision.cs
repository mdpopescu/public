namespace ExpressionCompiler.Models.Operators
{
    public class IntegerDivision : Operator<int>
    {
        public IntegerDivision()
            : base("/", 2, (a, b) => a / b)
        {
        }
    }
}