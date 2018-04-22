namespace Challenge4.Library.Models
{
    public class ElevatorInfo
    {
        public FloorInfo Floor1 { get; }
        public FloorInfo Floor2 { get; }
        public FloorInfo Floor3 { get; }

        public ElevatorInfo()
        {
            Floor1 = new FloorInfo();
            Floor2 = new FloorInfo();
            Floor3 = new FloorInfo();
        }
    }
}