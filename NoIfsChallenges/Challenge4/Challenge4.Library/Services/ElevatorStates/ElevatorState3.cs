using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState3 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState3()
        {
            Info = new ElevatorInfo();

            Info.Floor3.Button1Enabled = true;
            Info.Floor3.Button2Enabled = false;
            Info.Floor3.Button3Enabled = false;
            Info.Floor3.Screen = "Door Closed";

            Info.Floor2.Button1Enabled = true;
            Info.Floor2.Button2Enabled = false;
            Info.Floor2.Button3Enabled = false;
            Info.Floor2.Screen = "Door Closed";

            Info.Floor1.Button1Enabled = false;
            Info.Floor1.Button2Enabled = true;
            Info.Floor1.Button3Enabled = true;
            Info.Floor1.Screen = "Door Open";
        }

        public ElevatorState GoTo1st()
        {
            return new ElevatorState1();
        }

        public ElevatorState GoTo2nd()
        {
            return new ElevatorState2("");
        }

        public ElevatorState GoTo3rd()
        {
            return this;
        }

        public ElevatorState CallTo1st()
        {
            return new ElevatorState1();
        }

        public ElevatorState CallTo2nd()
        {
            return new ElevatorState2("");
        }

        public ElevatorState CallTo3rd()
        {
            return this;
        }
    }
}