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
            var elapsed = new Duration(Sys.Time() - timestamp);
            return $"{text} ({elapsed} ago)";
        }

        //

        private readonly string text;
        private readonly DateTime timestamp;
    }
}