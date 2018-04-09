using Challenge1.Library.Contracts;

namespace Challenge1.Library.Core
{
    public class PostOperatorState : CalculatorState
    {
        public string Display { get; private set; }

        public PostOperatorState(string previousDisplay, Operator op)
        {
            this.previousDisplay = previousDisplay;
            this.op = op;
        }

        public CalculatorState EnterDigit(char c)
        {
            Display += c.ToString();
            return this;
        }

        public CalculatorState EnterOperator(Operator _)
        {
            // do nothing
            return this;
        }

        public CalculatorState EnterEqual()
        {
            Display = op.Execute(previousDisplay, Display);
            return this;
        }

        //

        private readonly string previousDisplay;
        private readonly Operator op;
    }
}