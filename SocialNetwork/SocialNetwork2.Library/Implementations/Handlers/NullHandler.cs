using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;

namespace SocialNetwork2.Library.Implementations.Handlers
{
    public class NullHandler : IHandler
    {
        public static readonly NullHandler INSTANCE = new NullHandler();

        public IEnumerable<string> Handle(IUser user, string restOfInput)
        {
            return Enumerable.Empty<string>();
        }
    }
}