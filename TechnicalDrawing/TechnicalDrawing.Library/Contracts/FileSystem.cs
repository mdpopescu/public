using System.Collections.Generic;

namespace TechnicalDrawing.Library.Contracts
{
    public interface FileSystem
    {
        IEnumerable<string> ReadLines(string filename);
    }
}