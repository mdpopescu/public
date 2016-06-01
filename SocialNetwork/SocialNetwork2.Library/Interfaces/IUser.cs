using System.Collections.Generic;

namespace SocialNetwork2.Library.Interfaces
{
    public interface IUser
    {
        void Post(string message);
        IEnumerable<string> Read();
        void Follow(IUser otherUser);
        IEnumerable<string> Wall();
    }
}