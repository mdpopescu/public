using System.Collections.Generic;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Contracts
{
    public interface IMap
    {
        IEnumerable<Link> GetRoute(Endpoint origin, Endpoint destination);
    }
}