using Challenge4.Library.Contracts;
using Challenge4.Library.Models;
using Challenge4.Library.Services.ElevatorStates;

namespace Challenge4.Library.Services
{
    public class Elevator
    {
        public ElevatorInfo Info => state.Info;

        public Elevator()
        {
            state = new ElevatorState1();
        }

        public void GoTo1st()
        {
            //
        }

        public void GoTo2nd()
        {
            //
        }

        public void GoTo3rd()
        {
            //
        }

        public void CallTo1st()
        {
            //
        }

        public void CallTo2nd()
        {
            //
        }

        public void CallTo3rd()
        {
            //
        }

        //

        private readonly ElevatorState state;
    }
}