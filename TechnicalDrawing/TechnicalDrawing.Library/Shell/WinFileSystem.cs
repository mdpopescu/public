using System.Collections.Generic;
using System.IO;
using TechnicalDrawing.Library.Contracts;

namespace TechnicalDrawing.Library.Shell
{
    public class WinFileSystem : FileSystem
    {
        public IEnumerable<string> ReadLines(string filename) => File.ReadLines(filename);
    }
}