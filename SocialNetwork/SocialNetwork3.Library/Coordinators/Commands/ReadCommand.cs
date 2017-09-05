using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public class ReadCommand : Command
    {
        public ReadCommand(MessageRepository messages, MessageFormatter formatter)
        {
            this.messages = messages;
            this.formatter = formatter;
        }

        public override List<string> Execute(DateTime time, string user, string argument) =>
            messages
                .GetMessagesBy(user)
                .Select(m => formatter.Format(m, time))
                .ToList();

        //

        private readonly MessageRepository messages;
        private readonly MessageFormatter formatter;
    }
}