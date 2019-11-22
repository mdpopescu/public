using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class App
    {
        public App(IMap map, IEnumerable<Vehicle> vehicles)
        {
            this.map = map;
            this.vehicles = vehicles.ToArray();
        }

        public int Run(string origin, string[] destinations) => 0;

        //

        private readonly IMap map;
        private readonly Vehicle[] vehicles;
    }
}