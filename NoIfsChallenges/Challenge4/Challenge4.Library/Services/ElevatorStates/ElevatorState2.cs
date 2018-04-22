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

            Info.Floor3.Button1Enabled = true;
            Info.Floor3.Button2Enabled = false;
            Info.Floor3.Button3Enabled = false;
            Info.Floor3.Screen = Constants.DOOR_CLOSED;

            Info.Floor2.Button1Enabled = false;
            Info.Floor2.Button2Enabled = true;
            Info.Floor2.Button3Enabled = true;
            Info.Floor2.Screen = screen;

            Info.Floor1.Button1Enabled = true;
            Info.Floor1.Button2Enabled = false;
            Info.Floor1.Button3Enabled = false;
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