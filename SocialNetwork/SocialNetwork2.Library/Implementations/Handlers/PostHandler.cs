using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations.Handlers
{
    public class PostHandler : IHandler
    {
        public string KnownCommand => "->";

        public IEnumerable<string> Handle(IUser user, string restOfInput)
        {
            user.Post(restOfInput);
            return Enumerable.Empty<string>();
        }
    }
}