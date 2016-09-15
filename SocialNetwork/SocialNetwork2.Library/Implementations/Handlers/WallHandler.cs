using System.Collections.Generic;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations.Handlers
{
    public class WallHandler : IHandler
    {
        public IEnumerable<string> Handle(IUser user, string restOfInput)
        {
            return user.Wall();
        }
    }
}