using System.Collections.Generic;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Tests
{
    public class FakeCanvas : Canvas
    {
        public List<string> Commands { get; } = new List<string>();

        public void Execute(ProjectedCommand command) => Commands.Add(command.ToString());
    }
}