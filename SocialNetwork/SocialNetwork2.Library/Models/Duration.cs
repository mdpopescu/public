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

            if (hours > 0)
            {
                value = hours;
                unit = "hour";
            }
            else if (minutes > 0)
            {
                value = minutes;
                unit = "minute";
            }
            else
            {
                value = seconds;
                unit = "second";
            }

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