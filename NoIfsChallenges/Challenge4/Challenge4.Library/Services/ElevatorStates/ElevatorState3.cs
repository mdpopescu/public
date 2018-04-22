using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState3 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState3(string screen)
        {
            Info = new ElevatorInfo();

            Info.Floor3.CallEnabled = false;
            Info.Floor3.Screen = screen;

            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;

            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;
        }

        public ElevatorState GoTo1st() => new ElevatorState1(Constants.ELEVATOR_ARRIVES);
        public ElevatorState GoTo2nd() => new ElevatorState2(Constants.ELEVATOR_ARRIVES);
        public ElevatorState GoTo3rd() => this;

        public ElevatorState CallTo1st() => new ElevatorState1(Constants.ELEVATOR_CALLED_DOWN);
        public ElevatorState CallTo2nd() => new ElevatorState2(Constants.ELEVATOR_CALLED_DOWN);
        public ElevatorState CallTo3rd() => this;
    }
}