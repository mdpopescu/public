using StaticBlog.Library.Contracts;

namespace StaticBlog.Library.Implementations
{
    public class Authenticator
    {
        public Authenticator(SystemClock clock)
        {
            this.clock = clock;
        }

        public bool Login(string username, string password)
        {
            return true;
        }

        //

        private SystemClock clock;
    }
}