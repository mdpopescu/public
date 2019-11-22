using System.Collections.Generic;
using System.Linq;
using TransportTycoon.Library.Contracts;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Services
{
    public class Map : IMap
    {
        public Map(IEnumerable<Link> links)
        {
            this.links = links.ToArray();
        }

        //

        private readonly Link[] links;
    }
}