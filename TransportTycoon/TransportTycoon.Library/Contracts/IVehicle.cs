using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Contracts
{
    public interface IVehicle
    {
        void AddRoute(Endpoint e1, Endpoint e2, int cost);
    }
}