using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Interfaces;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Implementations
{
    public class User : IUser
    {
        public User(string userName)
        {
            this.userName = userName;
        }

        public void Post(string message)
        {
            messages.Push(new Message(userName, message));
        }

        public IEnumerable<string> Read()
        {
            return messages.Select(it => it.ToString());
        }

        public void Follow(IUser otherUser)
        {
            followedUsers.Add(otherUser);
        }

        public IEnumerable<string> Wall()
        {
            var allMessages = followedUsers
                .Concat(new[] { this })
                .SelectMany(user => user.GetMessages())
                .OrderByDescending(it => it);

            return allMessages.Select(it => it.ToTaggedString());
        }

        public IEnumerable<Message> GetMessages()
        {
            return messages.AsEnumerable();
        }

        //

        private readonly Stack<Message> messages = new Stack<Message>();
        private readonly List<IUser> followedUsers = new List<IUser>();

        private readonly string userName;
    }
}