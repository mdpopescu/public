using System.Collections.Generic;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Interfaces
{
    public interface IUser
    {
        void Post(string message);
        IEnumerable<string> Read();
        void Follow(IUser otherUser);
        IEnumerable<string> Wall();

        IEnumerable<Message> GetMessages();
    }
}