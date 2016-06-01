using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations.Handlers
{
    public class PostHandler : IHandler
    {
        public string KnownCommand => "->";

        public IEnumerable<string> Handle(IUser user, string[] restOfInput)
        {
            user.Post(string.Join(" ", restOfInput.Skip(2)));
            return Enumerable.Empty<string>();
        }
    }
}