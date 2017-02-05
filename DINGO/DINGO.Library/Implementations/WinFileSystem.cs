using System.Collections.Generic;
using System.IO;
using DINGO.Library.Contracts;

namespace DINGO.Library.Implementations
{
    public class WinFileSystem : FileSystem
    {
        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path, "*", SearchOption.AllDirectories);
    }
}