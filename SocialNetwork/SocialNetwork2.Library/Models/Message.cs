using System;
using SocialNetwork2.Library.Implementations;

namespace SocialNetwork2.Library.Models
{
    public class Message : IComparable<Message>
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

        public int CompareTo(Message other)
        {
            return timestamp < other.timestamp ? -1 : timestamp > other.timestamp ? 1 : 0;
        }

        //

        private readonly string text;
        private readonly DateTime timestamp;
    }
}