using System;
using System.Collections.Generic;
using SocialNetwork3.Library.Logic;

namespace SocialNetwork3.Library.Coordinators
{
    public class Network
    {
        /// <summary>Initializes a new instance of the <see cref="Network"/> class.</summary>
        /// <param name="parser">The message parser.</param>
        /// <param name="commandFactory">The command factory.</param>
        /// <param name="getTime">The "get current time" function.</param>
        public Network(MessageParser parser, CommandFactory commandFactory, Func<DateTime> getTime)
        {
            this.parser = parser;
            this.commandFactory = commandFactory;
            this.getTime = getTime;
        }

        /// <summary>
        /// Processes the specified line as if it were entered by an user.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The response lines, if any.</returns>
        /// <remarks>
        /// Posting or following does not return a response.
        /// Reading a user's messages or wall returns the appropriate messages, including the elapsed time since they were posted.
        /// </remarks>
        public IEnumerable<string> Enter(string line)
        {
            var parsedLine = parser.Parse(line);
            var command = commandFactory.Create(parsedLine.Command);
            return command.Execute(getTime(), parsedLine.User, parsedLine.Rest);
        }

        //

        private readonly MessageParser parser;
        private readonly CommandFactory commandFactory;
        private readonly Func<DateTime> getTime;
    }
}