using System.IO;
using System.Text;
using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class FileStore : ResourceStore<string>
    {
        public FileStore(string path)
        {
            this.path = path;
        }

        public string Load()
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public void Save(string value)
        {
            File.WriteAllText(path, value, Encoding.UTF8);
        }

        //

        private readonly string path;
    }
}