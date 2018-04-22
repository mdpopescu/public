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
            state = new ElevatorState1(Constants.DOOR_OPEN);
        }

        public void GoTo1st() => state = state.GoTo1st();
        public void GoTo2nd() => state = state.GoTo2nd();
        public void GoTo3rd() => state = state.GoTo3rd();

        public void CallTo1st() => state = state.CallTo1st();
        public void CallTo2nd() => state = state.CallTo2nd();
        public void CallTo3rd() => state = state.CallTo3rd();

        //

        private ElevatorState state;
    }
}