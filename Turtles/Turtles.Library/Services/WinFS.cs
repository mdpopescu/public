using Turtles.Library.Contracts;

namespace Turtles.Library.Services;

public class WinFS : IFileSystem
{
    public string Load(string filename) => File.ReadAllText(filename);
    public void Save(string filename, string text) => File.WriteAllText(filename, text);
}