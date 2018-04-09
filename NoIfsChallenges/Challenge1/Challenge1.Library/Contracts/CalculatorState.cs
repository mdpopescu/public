namespace Challenge1.Library.Contracts
{
    public interface CalculatorState
    {
        string Display { get; }

        CalculatorState EnterDigit(char c);
        CalculatorState EnterOperator(Operator op);
        CalculatorState EnterEqual();
    }
}