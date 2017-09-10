using System;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Shell
{
    public class DrawingApp
    {
        public DrawingApp(FileParser fileParser, Projector projector, Canvas canvas)
        {
            this.fileParser = fileParser;
            this.projector = projector;
            this.canvas = canvas;
        }

        public void Load(string filename)
        {
            var planes = Enum.GetValues(typeof(Plane)).Cast<Plane>().ToList();
            var parsedCommands = fileParser.Parse(filename).ToList();

            var groups = from plane in planes
                         from parsedCommand in parsedCommands
                         let command = CreateCommand(plane, parsedCommand)
                         group command by command.Plane;

            foreach (var g in groups)
                canvas.Execute(g.ToList());
        }

        //

        private readonly FileParser fileParser;
        private readonly Projector projector;
        private readonly Canvas canvas;

        /// <summary>Projects a command to the given plane.</summary>
        /// <param name="plane">The plane.</param>
        /// <param name="parsedCommand">The parsed command.</param>
        /// <returns>The command with its coordinates projected to the given plane as <see cref="Point2D"/>s.</returns>
        private ProjectedCommand CreateCommand(Plane plane, ParsedCommand parsedCommand) =>
            new ProjectedCommand(plane, parsedCommand.Name, projector.Project(plane, parsedCommand.Args));
    }
}