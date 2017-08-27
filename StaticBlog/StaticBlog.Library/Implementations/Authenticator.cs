using System;
using System.Linq;
using StaticBlog.Library.Contracts;
using StaticBlog.Library.Models;

namespace StaticBlog.Library.Implementations
{
    public class Authenticator
    {
        public Authenticator(SystemClock clock)
        {
            this.clock = clock;
        }

        public bool Login(Account account)
        {
            var result = Account.KnownAccounts.Any(it => it.Matches(account));

            if (result)
            {
                lastWaitTimeInSec = 1;
            }
            else
            {
                clock.Sleep(TimeSpan.FromSeconds(lastWaitTimeInSec));
                lastWaitTimeInSec = Math.Min(60, lastWaitTimeInSec * 2);
            }

            return result;
        }

        //

        private readonly SystemClock clock;

        private int lastWaitTimeInSec = 1;
    }
}