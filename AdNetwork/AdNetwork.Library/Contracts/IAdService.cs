using System.Threading.Tasks;
using AdNetwork.Library.Models;

namespace AdNetwork.Library.Contracts
{
    public interface IAdService
    {
        Task<AdResponse> GetOffer(Criteria criteria);
    }
}