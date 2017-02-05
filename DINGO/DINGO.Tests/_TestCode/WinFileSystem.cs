using System.Collections.Generic;
using System.IO;
using DINGO.Library.Contracts;

namespace DINGO.Library.Implementations
{
    public class WinFileSystem : FileSystem
    {
        /// <summary>
        /// Returns all files in the given path and its subfolders.
        /// </summary>
        public IEnumerable<string> GetFiles(string path) => Directory.GetFiles(path, "*", SearchOption.AllDirectories);
    }
}