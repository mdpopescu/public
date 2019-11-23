namespace TransportTycoon.Library.Models
{
    public class Vehicle
    {
        public Endpoint Location { get; private set; }

        public Vehicle(string id, Endpoint home)
        {
            this.id = id;
            this.home = home;

            Location = home;
        }

        public void MoveTo(Endpoint destination)
        {
            Location = destination;
        }

        //

        private readonly string id;
        private readonly Endpoint home;
    }
}