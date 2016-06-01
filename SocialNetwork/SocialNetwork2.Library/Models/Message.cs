using System;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Library.Models
{
    public class Message
    {
        public Message(string text)
        {
            this.text = text;
            timestamp = Sys.Time();
        }

        public override string ToString()
        {
            var elapsed = Formatted(Sys.Time() - timestamp);
            return $"{text} ({elapsed})";
        }

        //

        private readonly string text;
        private readonly DateTime timestamp;

        private static string Formatted(TimeSpan ts)
        {
            return ts.TotalSeconds + " seconds ago";
        }
    }
}