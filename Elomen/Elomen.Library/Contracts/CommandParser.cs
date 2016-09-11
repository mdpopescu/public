using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface CommandParser
    {
        Command Parse(string commandText);
    }
}