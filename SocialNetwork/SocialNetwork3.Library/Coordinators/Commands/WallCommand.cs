using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork3.Library.Logic;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public class WallCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="WallCommand"/> class.</summary>
        /// <param name="messages">The message repository.</param>
        /// <param name="users">The user repository.</param>
        /// <param name="formatter">The message formatter.</param>
        public WallCommand(MessageRepository messages, UserRepository users, MessageFormatter formatter)
        {
            this.messages = messages;
            this.users = users;
            this.formatter = formatter;
        }

        /// <inheritdoc />
        public override List<string> Execute(DateTime time, string user, string argument)
        {
            var list = users.GetFollowed(user).Concat(new[] { user }).ToList();
            return messages
                .GetMessagesBy(list)
                .Select(m => IncludeSender(m, time))
                .ToList();
        }

        //

        private readonly MessageRepository messages;
        private readonly UserRepository users;
        private readonly MessageFormatter formatter;

        private string IncludeSender(Message m, DateTime time) => m.User + " - " + formatter.Format(m, time);
    }
}