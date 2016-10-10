using Renfield.FSM.Library;

namespace Renfield.FSM.ParseNumbers
{
    public class FSMParser
    {
        public FSMParser()
        {
            fsm = new FSM<char>("start");
            SetUp();
        }

        public double Parse(string s)
        {
            fsm.Restart();
            foreach (var c in s)
                fsm.Handle(c);

            number *= sign;

            return number;
        }

        //

        private int sign;
        private double number;
        private double divisor;

        private readonly FSM<char> fsm;

        private void SetUp()
        {
            fsm.OnStart = () =>
            {
                sign = 1;
                number = 0.0;
                divisor = 0.0;
            };

            fsm.Add("start", '+', null, "whole");
            fsm.Add("start", '-', _ => sign = -1, "whole");
            fsm.Add("start", char.IsDigit, AddChar, "whole");
            fsm.Add("start", '.', _ => divisor = 0.1, "fract");

            fsm.Add("whole", char.IsDigit, AddChar);
            fsm.Add("whole", '.', _ => divisor = 0.1, "fract");

            fsm.Add("fract", char.IsDigit, c =>
            {
                number += divisor * (c - '0');
                divisor /= 10;
            });
        }

        private void AddChar(char c)
        {
            number = number * 10 + (c - '0');
        }
    }
}