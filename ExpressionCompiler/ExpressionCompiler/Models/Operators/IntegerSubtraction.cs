namespace ExpressionCompiler.Models.Operators
{
    public class IntegerSubtraction : Operator<int>
    {
        public IntegerSubtraction()
            : base("-", 1, (a, b) => a - b)
        {
        }

        public override object Clone() => new IntegerSubtraction();
    }
}