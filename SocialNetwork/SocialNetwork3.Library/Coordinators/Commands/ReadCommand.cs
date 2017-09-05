using System;
using System.Collections.Generic;
using System.Linq;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public class ReadCommand : Command
    {
        /// <summary>Initializes a new instance of the <see cref="ReadCommand"/> class.</summary>
        /// <param name="messages">The message repository.</param>
        /// <param name="formatter">The message formatter.</param>
        public ReadCommand(MessageRepository messages, MessageFormatter formatter)
        {
            this.messages = messages;
            this.formatter = formatter;
        }

        /// <inheritdoc />
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