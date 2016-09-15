using System.Collections.Generic;

namespace SocialNetwork2.Library.Interfaces
{
    public interface IHandler
    {
        IEnumerable<string> Handle(IUser user, string restOfInput);
    }
}