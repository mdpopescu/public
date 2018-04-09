using Challenge1.Library.Contracts;

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
            display += c;
            ui.Display(display);
        }

        public void EnterOperator(char c)
        {
            //
        }

        public void EnterEqual()
        {
            //
        }

        //

        private readonly MainUI ui;

        private string display = "";
    }
}