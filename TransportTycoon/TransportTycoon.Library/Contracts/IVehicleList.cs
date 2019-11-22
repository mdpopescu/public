using TransportTycoon.Library.Models;

namespace TransportTycoon.Library.Contracts
{
    public interface IVehicleList
    {
        Vehicle FindAvailable(Endpoint location);
    }
}