using System;
using SocialNetwork3.Library.Models;

namespace SocialNetwork3.Library.Logic
{
    public class MessageFormatter
    {
        /// <summary>Formats the specified message.</summary>
        /// <param name="message">The message.</param>
        /// <param name="time">The current time.</param>
        /// <returns>The message text plus the elapsed time in minutes or seconds.</returns>
        public string Format(Message message, DateTime time)
        {
            var elapsed = time - message.Time;
            var minutes = (int) Math.Truncate(elapsed.TotalMinutes);
            var seconds = (int) Math.Truncate(elapsed.TotalSeconds);

            string s;

            if (minutes > 59)
                s = "long";
            else if (minutes > 1)
                s = $"{minutes} minutes";
            else if (minutes == 1)
                s = "1 minute";
            else if (seconds != 1)
                s = $"{seconds} seconds";
            else
                s = "1 second";

            return $"{message.Text} ({s} ago)";
        }
    }
}