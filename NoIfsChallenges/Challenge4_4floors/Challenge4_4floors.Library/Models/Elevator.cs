namespace Challenge4_4floors.Library.Models
{
    public class Elevator
    {
        public bool IsLit1 { get; private set; }
        public bool IsLit2 { get; private set; }
        public bool IsLit3 { get; private set; }
        public bool IsLit4 { get; private set; }
        public string Display { get; private set; }

        public Elevator()
        {
            IsLit1 = false;
            IsLit2 = false;
            IsLit3 = false;
            IsLit4 = false;
            Display = Constants.DOOR_CLOSED;
        }
    }
}