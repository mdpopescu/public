using System;

namespace SocialNetwork2.Library.Models
{
    public class Duration
    {
        public Duration(TimeSpan ts)
        {
            var hours = (int) Math.Truncate(ts.TotalHours);
            var minutes = (int) Math.Truncate(ts.TotalMinutes);
            var seconds = (int) Math.Truncate(ts.TotalSeconds);

            value = hours > 0 ? hours : minutes > 0 ? minutes : seconds;
            unit = hours > 0 ? "hour" : minutes > 0 ? "minute" : "second";

            suffix = value == 1 ? "" : "s";
        }

        public override string ToString()
        {
            return $"{value} {unit}{suffix}";
        }

        //

        private readonly int value;
        private readonly string unit;
        private readonly string suffix;
    }
}