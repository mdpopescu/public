using System;
using System.Linq;

namespace ETL.Library.Phases.Phase1
{
    public class InputSpecParser
    {
        public InputSpec Parse(string spec)
        {
            var fields = spec
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(ParseLine)
                .ToList();

            return new InputSpec { Fields = fields };
        }

        //

        private static InputField ParseLine(string line)
        {
            var parts = line.Split(',');
            return new InputField { Name = GetName(parts[0]), Size = GetSize(parts[1]) };
        }

        private static string GetName(string s)
        {
            return s.Substring(1, s.Length - 2).Replace(' ', '_');
        }

        private static int GetSize(string s)
        {
            return int.Parse(s);
        }
    }
}