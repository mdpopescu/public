using System;
using System.Collections.Generic;
using SocialNetwork3.Library.Coordinators.Commands;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators
{
    public class CommandFactory
    {
        public CommandFactory(MessageRepository messages, MessageFormatter formatter)
        {
            MAP.Add("->", () => new PostCommand(messages));
            MAP.Add("", () => new ReadCommand(messages, formatter));
        }

        public Command Create(string command)
        {
            return MAP[command].Invoke();
        }

        //

        private static readonly Dictionary<string, Func<Command>> MAP = new Dictionary<string, Func<Command>>();
    }
}