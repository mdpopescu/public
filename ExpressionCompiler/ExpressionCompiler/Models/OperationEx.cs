namespace ExpressionCompiler.Models
{
    public class OperationEx : Operation
    {
        public int Index { get; }

        public OperationEx(int index, Operation operation)
            : base(operation.Symbol, operation.Priority)
        {
            Index = index;
        }
    }
}