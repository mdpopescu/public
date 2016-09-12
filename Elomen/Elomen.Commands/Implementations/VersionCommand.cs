using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.Commands.Implementations
{
    public class VersionCommand : Command
    {
        public string Execute(Account account)
        {
            return "Version 0.01";
        }
    }
}