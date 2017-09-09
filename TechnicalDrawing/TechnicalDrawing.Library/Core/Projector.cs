using System;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Core
{
    public class Projector
    {
        public ProjectedCommand Project(ParsedCommand command, Quadrant quadrant)
        {
            return new ProjectedCommand(command.Name, quadrant, new QuadrantPoint((int) Math.Truncate(command.Args[0]), (int) Math.Truncate(command.Args[1])));
        }
    }
}