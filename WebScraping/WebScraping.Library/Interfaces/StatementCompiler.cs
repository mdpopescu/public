namespace WebScraping.Library.Interfaces
{
    public interface StatementCompiler
    {
        bool CanHandle(string[] statement);
        string[] Compile(string[] statement);
    }
}