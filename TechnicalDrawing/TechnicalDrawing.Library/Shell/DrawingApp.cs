using System;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Shell
{
    public class DrawingApp
    {
        public DrawingApp(FileSystem fs, Parser parser, Projector projector, Canvas canvas)
        {
            this.fs = fs;
            this.parser = parser;
            this.projector = projector;
            this.canvas = canvas;
        }

        public void Load(string filename)
        {
            var parsedLines = fs
                .ReadLines(filename)
                .Select(parser.Parse)
                .ToList();
            var commands = from parsedLine in parsedLines
                           from quadrant in Enum.GetValues(typeof(Quadrant)).Cast<Quadrant>()
                           select projector.Project(parsedLine, quadrant);

            foreach (var command in commands)
                command.Execute(canvas);
        }

        //

        private readonly FileSystem fs;
        private readonly Parser parser;
        private readonly Projector projector;
        private readonly Canvas canvas;
    }
}