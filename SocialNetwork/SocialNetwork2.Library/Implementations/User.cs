using System.Collections.Generic;
using System.Linq;

namespace SocialNetwork2.Library.Implementations
{
    public class User
    {
        public User(string userName)
        {
            //
        }

        public void Post(string message)
        {
            messages.Add(message);
        }

        public IEnumerable<string> Read()
        {
            return messages.AsEnumerable();
        }

        public void Follow(string otherUser)
        {
            //
        }

        public IEnumerable<string> Wall()
        {
            return Enumerable.Empty<string>();
        }

        //

        private readonly List<string> messages = new List<string>();
    }
}