namespace Turtles.Library.Contracts;

public interface IFileSystem
{
    string Load(string filename);
    void Save(string filename, string text);
}