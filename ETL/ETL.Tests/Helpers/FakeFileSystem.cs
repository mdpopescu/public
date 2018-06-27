using System.Collections.Generic;
using ETL.Library.Contracts;

namespace ETL.Tests.Helpers
{
    public class FakeFileSystem : FileSystem
    {
        public Dictionary<string, string> Files { get; } = new Dictionary<string, string>();

        public void SaveFile(string filename, string content)
        {
            Files[filename] = content;
        }
    }
}