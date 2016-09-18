using Elomen.Library.Contracts;
using Elomen.TwitterLibrary.Models;

namespace Elomen.Spike.Implementations
{
    public class NullAccountRepository : Repository<long, TwitterAccount>
    {
        public TwitterAccount Find(long id)
        {
            return new TwitterAccount(0, "");
        }
    }
}