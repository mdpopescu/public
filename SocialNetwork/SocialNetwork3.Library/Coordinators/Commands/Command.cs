using System;
using System.Collections.Generic;

namespace SocialNetwork3.Library.Coordinators.Commands
{
    public abstract class Command
    {
        /// <summary>Executes the command.</summary>
        /// <param name="time">The time when the command was sent.</param>
        /// <param name="user">The user who sent the command.</param>
        /// <param name="argument">The argument, if any.</param>
        /// <returns>The response, if any.</returns>
        public abstract List<string> Execute(DateTime time, string user, string argument);
    }
}