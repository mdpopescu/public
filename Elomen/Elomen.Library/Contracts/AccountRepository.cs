using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface AccountRepository
    {
        Account Find(string accountId);
    }
}