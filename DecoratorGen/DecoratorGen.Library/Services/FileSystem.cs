using System.IO;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class FileSystem : IFileSystem
    {
        public FileSystem(string rootFolder)
        {
            this.rootFolder = rootFolder;
        }

        public string ReadText(string filename) =>
            File.ReadAllText(Path.Combine(rootFolder, filename));

        public void WriteText(string filename, string text) =>
            File.WriteAllText(Path.Combine(rootFolder, filename), text);

        //

        private readonly string rootFolder;
    }
}