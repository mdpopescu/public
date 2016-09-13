namespace Elomen.Storage.Contracts
{
    public interface FileSystem
    {
        string Load(string path);
        void Save(string path, string contents);
    }
}