using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork2.Library.Implementations
{
    public class CommandHandler
    {
        public CommandHandler(UserRepository userRepository)
        {
            //
        }

        public IEnumerable<string> Handle(string command)
        {
            return Enumerable.Empty<string>();
        }
    }
}