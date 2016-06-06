using System.IO;
using System.Text;
using TweetNicer.Library.Interfaces;

namespace TweetNicer.Library.Implementations
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