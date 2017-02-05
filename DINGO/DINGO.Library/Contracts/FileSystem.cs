using System.Collections.Generic;

namespace DINGO.Library.Contracts
{
    public interface FileSystem
    {
        /// <summary>
        /// Returns all files in the given path and its subfolders.
        /// </summary>
        IEnumerable<string> GetFiles(string path);
    }
}