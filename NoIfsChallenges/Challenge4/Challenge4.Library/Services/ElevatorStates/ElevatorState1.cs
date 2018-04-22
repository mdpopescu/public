using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState1 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState1(string screen)
        {
            Info = new ElevatorInfo();

            Info.Floor3.CallEnabled = true;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;

            Info.Floor2.CallEnabled = true;
            Info.Floor2.Screen = Constants.DOOR_CLOSED;

            Info.Floor1.CallEnabled = false;
            Info.Floor1.Screen = screen;
        }

        public ElevatorState GoTo1st() => this;
        public ElevatorState GoTo2nd() => new ElevatorState2(Constants.ELEVATOR_ARRIVES);
        public ElevatorState GoTo3rd() => new ElevatorState3(Constants.ELEVATOR_ARRIVES);

        public ElevatorState CallTo1st() => this;
        public ElevatorState CallTo2nd() => new ElevatorState2(Constants.ELEVATOR_CALLED_UP);
        public ElevatorState CallTo3rd() => new ElevatorState3(Constants.ELEVATOR_CALLED_UP);
    }
}