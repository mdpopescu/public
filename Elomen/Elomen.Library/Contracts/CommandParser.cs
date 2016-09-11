namespace Elomen.Library.Contracts
{
    public interface CommandParser
    {
        Command Parse(string commandText);
    }
}