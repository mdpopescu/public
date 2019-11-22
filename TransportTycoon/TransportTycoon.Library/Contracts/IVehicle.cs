using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Contracts
{
    public interface IVehicle
    {
        void SetCost(Link link, int cost);
    }
}