using Elomen.Library.Contracts;
using Elomen.Library.Model;

namespace Elomen.Library.Implementations
{
    public class Interpreter : Executable
    {
        public Interpreter(CommandParser commandParser)
        {
            this.commandParser = commandParser;
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="account">The account of the user sending the command.</param>
        /// <param name="commandText">The (natural language) command.</param>
        /// <returns>The result of executing the command (or an error message).</returns>
        public string Execute(Account account, string commandText)
        {
            var command = commandParser.Parse(commandText);

            return command?.Execute(account)
                   ?? $"I do not know what [{commandText}] means.";
        }

        //

        private readonly CommandParser commandParser;
    }
}