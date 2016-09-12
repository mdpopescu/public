using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.Tester.Implementations
{
    public class NullAccountRepository : AccountRepository
    {
        public Account Find(string accountId)
        {
            return Account.GUEST;
        }
    }
}