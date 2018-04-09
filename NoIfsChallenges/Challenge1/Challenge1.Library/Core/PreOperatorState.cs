using Challenge1.Library.Contracts;

namespace Challenge1.Library.Core
{
    public class PreOperatorState : CalculatorState
    {
        public string Display { get; private set; }

        public PreOperatorState()
        {
            Display = "";
        }

        public CalculatorState EnterDigit(char c)
        {
            Display += c.ToString();
            return this;
        }

        public CalculatorState EnterOperator(Operator op)
        {
            return new PostOperatorState(Display, op);
        }

        public CalculatorState EnterEqual()
        {
            // do nothing
            return this;
        }
    }
}