using System.Collections.Generic;
using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Contracts
{
    public interface IMap
    {
        void AddEndpoints(IEnumerable<Endpoint> endpoints);
    }
}