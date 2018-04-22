using Challenge4.Library.Models;

namespace Challenge4.Library.Services
{
    public class Elevator
    {
        public ElevatorInfo ElevatorInfo { get; }

        public Elevator()
        {
            ElevatorInfo = new ElevatorInfo();
        }

        public void Initialize()
        {
            ElevatorInfo.Floor3.Button1Enabled = true;
            ElevatorInfo.Floor3.Button2Enabled = false;
            ElevatorInfo.Floor3.Button3Enabled = false;
            ElevatorInfo.Floor3.Screen = "Door Closed";

            ElevatorInfo.Floor2.Button1Enabled = true;
            ElevatorInfo.Floor2.Button2Enabled = false;
            ElevatorInfo.Floor2.Button3Enabled = false;
            ElevatorInfo.Floor2.Screen = "Door Closed";

            ElevatorInfo.Floor1.Button1Enabled = false;
            ElevatorInfo.Floor1.Button2Enabled = true;
            ElevatorInfo.Floor1.Button3Enabled = true;
            ElevatorInfo.Floor1.Screen = "Door Open";
        }
    }
}