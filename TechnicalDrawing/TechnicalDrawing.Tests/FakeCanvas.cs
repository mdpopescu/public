using System.Collections.Generic;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests
{
    public class FakeCanvas : Canvas
    {
        public List<string> Commands { get; } = new List<string>();

        public void Execute(IEnumerable<ProjectedCommand> commands) => Commands.AddRange(commands.Select(it => it.ToString()));
    }
}