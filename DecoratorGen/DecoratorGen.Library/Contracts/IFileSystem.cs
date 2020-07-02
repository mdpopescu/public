namespace DecoratorGen.Library.Contracts
{
    public interface IFileSystem
    {
        string ReadText(string filename);
        void WriteText(string filename, string text);
    }
}