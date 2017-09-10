using System.Collections.Generic;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests
{
    public class FakeCanvas : Canvas
    {
        public List<List<string>> Commands { get; } = new List<List<string>>();

        public void Execute(IReadOnlyCollection<ProjectedCommand> commands) => Commands.Add(new List<string>(commands.Select(it => it.ToString())));
    }
}