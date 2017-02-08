namespace ExpressionCompiler.Models.Operands
{
    public class Integer : Operand<int>
    {
        public Integer(string value)
            : base(value)
        {
            ActualValue = int.Parse(value);
        }

        public Integer(int value)
            : base(value.ToString())
        {
            ActualValue = value;
        }

        public override int ActualValue { get; }
    }
}