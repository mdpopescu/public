using System;
using System.Linq;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageParser
    {
        /// <summary>Parses the specified line.</summary>
        /// <param name="line">The line.</param>
        /// <returns>The parsed line.</returns>
        public ParsedLine Parse(string line)
        {
            return KNOWN_COMMANDS
                    .Select(it => TryParsing(line, it))
                    .Where(it => it != null)
                    .FirstOrDefault()
                ?? new ParsedLine(line, "", null);
        }

        //

        private static readonly string[] KNOWN_COMMANDS = { "->", "follows", "wall" };

        private static ParsedLine TryParsing(string line, string command)
        {
            var index = line.IndexOf(command, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return null;

            var user = line.Substring(0, index).Trim();
            var text = line.Substring(index + command.Length).TrimStart();
            return new ParsedLine(user, command.ToUpperInvariant(), text);
        }
    }
}