using TransportTycoon.Library.Contracts;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map)
        {
            this.map = map;
        }

        public void AddVehicle(IVehicle vehicle, string id)
        {
            throw new System.NotImplementedException();
        }

        public int Run(string containers) => 0;

        //

        private readonly IMap map;
    }
}