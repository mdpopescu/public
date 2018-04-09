using Challenge1.Library.Contracts;
using Challenge1.Library.Core;

namespace Challenge1.Library.Shell
{
    public class MainLogic
    {
        public MainLogic(MainUI ui)
        {
            this.ui = ui;
        }

        public void EnterDigit(char c)
        {
            state = state.EnterDigit(c);
            ui.Display(state.Display);
        }

        public void EnterOperator(Operator op)
        {
            state = state.EnterOperator(op);
        }

        public void EnterEqual()
        {
            state = state.EnterEqual();
            ui.Display(state.Display);
            state = new PreOperatorState();
        }

        //

        private readonly MainUI ui;

        private CalculatorState state = new PreOperatorState();
    }
}