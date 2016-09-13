using System.IO;
using System.Text;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class WindowsFileSystem : FileSystem
    {
        public string Load(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public void Save(string path, string contents)
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
        }
    }
}