namespace ETL.Library.Contracts
{
    public interface FileSystem
    {
        void SaveFile(string filename, string content);
    }
}