using Elomen.Storage.Contracts;

namespace Elomen.Storage.Implementations
{
    public class FileStore : ResourceStore<string>
    {
        public FileStore(FileSystem fs, string path)
        {
            this.fs = fs;
            this.path = path;
        }

        public string Load()
        {
            return fs.Load(path);
        }

        public void Save(string value)
        {
            fs.Save(path, value);
        }

        //

        private readonly FileSystem fs;
        private readonly string path;
    }
}