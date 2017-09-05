using System;
using System.Collections.Generic;
using SocialNetwork3.Library.Coordinators.Commands;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators
{
    public class CommandFactory
    {
        /// <summary>Initializes a new instance of the <see cref="CommandFactory"/> class.</summary>
        /// <param name="messages">The message repository.</param>
        /// <param name="users">The user repository.</param>
        /// <param name="formatter">The message formatter.</param>
        public CommandFactory(MessageRepository messages, UserRepository users, MessageFormatter formatter)
        {
            MAP.Add("->", () => new PostCommand(messages));
            MAP.Add("FOLLOWS", () => new FollowsCommand(users));
            MAP.Add("WALL", () => new WallCommand(messages, users, formatter));
            MAP.Add("", () => new ReadCommand(messages, formatter));
        }

        /// <summary>Creates the specified command.</summary>
        /// <param name="command">The command name.</param>
        /// <returns>The appropriate command object.</returns>
        public Command Create(string command) => MAP[command].Invoke();

        //

        private static readonly Dictionary<string, Func<Command>> MAP = new Dictionary<string, Func<Command>>();
    }
}