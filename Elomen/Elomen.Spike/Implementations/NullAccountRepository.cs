using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.Spike.Implementations
{
    public class NullAccountRepository : AccountRepository
    {
        public Account Find(long accountId)
        {
            return Account.GUEST;
        }
    }
}