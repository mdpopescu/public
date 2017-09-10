using System;
using System.Linq;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class LineParser
    {
        public ParsedCommand Parse(string line)
        {
            var index = line.IndexOf('#');
            if (index >= 0)
                line = line.Remove(index);

            if (string.IsNullOrWhiteSpace(line))
                return new ParsedCommand(CommandName.None);

            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new ParsedCommand(CommandName.Line, parts.Skip(1).Select(float.Parse).ToArray());
        }
    }
}