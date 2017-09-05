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

        /// <summary>Gets the messages sent by the given users.</summary>
        /// <param name="list">The list of users.</param>
        /// <returns>The list of messages sent by the given users.</returns>
        /// <remarks>The list of messages is sorted by date/time in descending order.</remarks>
        public IEnumerable<Message> GetMessagesBy(IList<string> list) => messages
            .Where(it => list.Contains(it.User, StringComparer.OrdinalIgnoreCase))
            .OrderByDescending(it => it.Time);

        //

        private readonly List<Message> messages = new List<Message>();
    }
}