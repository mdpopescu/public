using System;

namespace SocialNetwork3.Library.Models
{
    public class Message
    {
        public Message(DateTime time, string user, string text)
        {
            Time = time;
            User = user;
            Text = text;
        }

        public DateTime Time { get; }
        public string User { get; }
        public string Text { get; }
    }
}