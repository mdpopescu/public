using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState2 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState2(string screen)
        {
            Info = new ElevatorInfo();

            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;

            Info.Floor2.CallEnabled = false;
            Info.Floor2.Screen = screen;

            Info.Floor1.CallEnabled = true;
            Info.Floor1.Screen = Constants.DOOR_CLOSED;
        }

        public ElevatorState GoTo1st() => new ElevatorState1(Constants.ELEVATOR_ARRIVES);
        public ElevatorState GoTo2nd() => this;
        public ElevatorState GoTo3rd() => new ElevatorState3(Constants.ELEVATOR_ARRIVES);

        public ElevatorState CallTo1st() => new ElevatorState1(Constants.ELEVATOR_CALLED_DOWN);
        public ElevatorState CallTo2nd() => this;
        public ElevatorState CallTo3rd() => new ElevatorState3(Constants.ELEVATOR_CALLED_UP);
    }
}