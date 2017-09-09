using System;
using System.Linq;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class Parser
    {
        public ParsedLine Parse(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return new ParsedLine(parts[0], parts.Skip(1).Select(float.Parse).ToArray());
        }
    }
}