using Elomen.Library.Model;

namespace Elomen.Library.Contracts
{
    public interface Command
    {
        /// <summary>
        /// Executes a command and returns the result.
        /// </summary>
        /// <param name="account">The account of the user invoking the command.</param>
        /// <returns>The result of the command.</returns>
        string Execute(Account account);
    }
}