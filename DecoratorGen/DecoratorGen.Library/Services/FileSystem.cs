using System.IO;
using DecoratorGen.Library.Contracts;

namespace DecoratorGen.Library.Services
{
    public class FileSystem : IFileSystem
    {
        public string ReadText(string filename) =>
            File.ReadAllText(filename);

        public void WriteText(string filename, string text) =>
            File.WriteAllText(filename, text);
    }
}