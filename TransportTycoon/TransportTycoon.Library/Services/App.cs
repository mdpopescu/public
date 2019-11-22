using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map)
        {
            this.map = map;
        }

        public void AddVehicle(IVehicle vehicle, string id, Endpoint startingEndpoint)
        {
            // 
        }

        public int Run(string origin, string[] destinations) => 0;

        //

        private readonly IMap map;
    }
}