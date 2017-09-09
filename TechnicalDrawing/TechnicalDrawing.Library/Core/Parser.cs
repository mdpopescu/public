using System;
using System.Linq;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class Parser
    {
        public ParsedCommand Parse(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new ParsedCommand(CommandName.Line, parts.Skip(1).Select(float.Parse).ToArray());
        }
    }
}