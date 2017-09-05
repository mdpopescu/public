using System;
using System.Collections.Generic;
using System.Diagnostics;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageProcessor
    {
        /// <summary>Processes the line and either adds a new message or returns a response.</summary>
        /// <param name="line">The line.</param>
        /// <param name="time">The current time.</param>
        /// <param name="add">The callback for adding a new message.</param>
        /// <returns>A response (which can be empty).</returns>
        public IList<string> ProcessLine(string line, DateTime time, Action<Message> add)
        {
            Debug.Assert(!string.IsNullOrEmpty(line));

            TryToProcessPost(line, time, add);

            return new List<string>();
        }

        private static void TryToProcessPost(string line, DateTime time, Action<Message> add)
        {
            var index = line.IndexOf("->", StringComparison.Ordinal);
            if (index < 0)
                return;

            var user = line.Substring(0, index).Trim();
            var text = line.Substring(index + 2).TrimStart();

            add(new Message(time, user, text));
        }
    }
}