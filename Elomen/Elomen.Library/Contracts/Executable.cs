namespace Elomen.Library.Contracts
{
    public interface Executable
    {
        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="accountId">The account of the user sending the command.</param>
        /// <param name="commandText">The (natural language) command.</param>
        /// <returns>The result of executing the command (or an error message).</returns>
        string Execute(long accountId, string commandText);
    }
}