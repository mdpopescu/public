using System.Collections.Generic;
using System.Linq;
using TechnicalDrawing.Library.Contracts;
using TechnicalDrawing.Library.Core;
using TechnicalDrawing.Library.Models;

namespace TechnicalDrawing.Library.Shell
{
    public class FileParser
    {
        public FileParser(FileSystem fs, LineParser lineParser)
        {
            this.fs = fs;
            this.lineParser = lineParser;
        }

        /// <summary>Parses the specified filename.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The parsed commands.</returns>
        public IEnumerable<ParsedCommand> Parse(string filename) =>
            fs
                .ReadLines(filename)
                .Select(lineParser.Parse)
                .Where(it => it.Name != CommandName.None)
                .ToList();

        //

        private readonly FileSystem fs;
        private readonly LineParser lineParser;
    }
}