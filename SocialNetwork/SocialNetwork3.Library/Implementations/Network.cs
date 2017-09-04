using System;
using System.Collections.Generic;

namespace SocialNetwork3.Library.Implementations
{
    public class Network
    {
        public Network(MessageProcessor processor, MessageRepository messages, Func<DateTime> getTime)
        {
            this.processor = processor;
            this.messages = messages;
            this.getTime = getTime;
        }

        /// <summary>
        /// Enters the specified line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>The response lines, if any.</returns>
        /// <remarks>
        /// Posting or following does not return a response.
        /// Reading a user's messages or wall returns the appropriate messages together with the elapsed time since they were posted.
        /// </remarks>
        public IEnumerable<string> Enter(string line)
        {
            return processor.ProcessLine(line, getTime(), messages.Add);
        }

        //

        private readonly MessageProcessor processor;
        private readonly MessageRepository messages;
        private readonly Func<DateTime> getTime;
    }
}