using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Implementations
{
    public class User
    {
        public User(string userName)
        {
            this.userName = userName;
        }

        public void Post(string message)
        {
            messages.Push(new Message(message));
        }

        public IEnumerable<string> Read()
        {
            return messages.Select(it => it.ToString());
        }

        public void Follow(User otherUser)
        {
            followedUsers.Add(otherUser);
        }

        public IEnumerable<string> Wall()
        {
            var allMessages = followedUsers
                .Concat(new[] { this })
                .SelectMany(user => user.messages.Select(message => new { user, message }))
                .OrderByDescending(it => it.message);

            return allMessages.Select(it => it.user.userName + " - " + it.message.ToString());
        }

        //

        private readonly Stack<Message> messages = new Stack<Message>();
        private readonly List<User> followedUsers = new List<User>();

        private readonly string userName;
    }
}