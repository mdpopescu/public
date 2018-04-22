using Challenge4.Library.Contracts;
using Challenge4.Library.Models;

namespace Challenge4.Library.Services.ElevatorStates
{
    public class ElevatorState1 : ElevatorState
    {
        public ElevatorInfo Info { get; }

        public ElevatorState1()
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
    }
}