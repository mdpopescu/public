using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageRepository
    {
        /// <summary>Adds the specified message.</summary>
        /// <param name="message">The message.</param>
        public void Add(Message message) => messages.Add(message);

        /// <summary>Gets the messages sent by the given user.</summary>
        /// <param name="user">The user.</param>
        /// <returns>The list of messages sent by the given user.</returns>
        public IEnumerable<Message> GetMessagesBy(string user) => messages.Where(it => string.Equals(it.User, user, StringComparison.OrdinalIgnoreCase));

        //

        private readonly List<Message> messages = new List<Message>();
    }
}