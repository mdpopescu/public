using Elomen.Commands.Implementations;
using Elomen.Library.Contracts;

namespace Elomen.Spike.Implementations
{
    public class FakeCommandParser : CommandParser
    {
        public Command Parse(string commandText)
        {
            return commandText.ToLowerInvariant() == "version" ? new VersionCommand() : null;
        }
    }
}