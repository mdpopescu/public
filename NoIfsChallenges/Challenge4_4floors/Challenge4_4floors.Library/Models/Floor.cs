namespace Challenge4_4floors.Library.Models
{
    public class Floor
    {
        public bool HasElevator { get; }
        public string Display { get; }

        public Floor(bool hasElevator, string display)
        {
            HasElevator = hasElevator;
            Display = display;
        }
    }
}