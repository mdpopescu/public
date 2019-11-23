using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map, IVehicleList vehicles)
        {
            this.map = map;
            this.vehicles = vehicles;
        }

        public int Run(Endpoint origin, IEnumerable<Endpoint> destinations)
        {
            var total = 0;
            foreach (var destination in destinations)
            {
                // assume a route exists from origin to destination
                var route = map.GetRoute(origin, destination).ToArray();

                foreach (var link in route)
                {
                    // assume that a vehicle is available at the start of the link
                    var vehicle = vehicles.FindAvailable(link.E1);
                    vehicle.MoveTo(link.E2);

                    total += link.Cost;
                }
            }

            return total;
        }

        //

        private readonly IMap map;
        private readonly IVehicleList vehicles;
    }
}