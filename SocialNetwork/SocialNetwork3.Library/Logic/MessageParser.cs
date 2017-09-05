using System;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageParser
    {
        public ParsedLine Parse(string line)
        {
            return TryParsingPost(line)
                ?? TryParsingRead(line);
        }

        //

        private static ParsedLine TryParsingPost(string line)
        {
            var index = line.IndexOf("->", StringComparison.Ordinal);
            if (index < 0)
                return null;

            var user = line.Substring(0, index).Trim();
            var text = line.Substring(index + 2).TrimStart();
            return new ParsedLine(user, "->", text);
        }

        private static ParsedLine TryParsingRead(string line)
        {
            return new ParsedLine(line, "", null);
        }
    }
}