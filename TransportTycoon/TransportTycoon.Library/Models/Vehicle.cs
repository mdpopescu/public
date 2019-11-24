namespace TransportTycoon.Library.Models
{
    public class Vehicle
    {
        public Endpoint Location { get; private set; }

        public Vehicle(Endpoint home)
        {
            Location = home;
        }

        public void MoveTo(Endpoint destination)
        {
            Location = destination;
        }
    }
}