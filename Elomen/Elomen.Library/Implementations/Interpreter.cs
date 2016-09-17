using Elomen.Library.Contracts;

namespace Elomen.Library.Implementations
{
    public class Interpreter : Executable
    {
        public Interpreter(AccountRepository accountRepository, CommandParser commandParser)
        {
            this.accountRepository = accountRepository;
            this.commandParser = commandParser;
        }

        /// <summary>
        /// Executes a command.
        /// </summary>
        /// <param name="accountId">The account of the user sending the command.</param>
        /// <param name="commandText">The (natural language) command.</param>
        /// <returns>The result of executing the command (or an error message).</returns>
        public string Execute(string accountId, string commandText)
        {
            var account = accountRepository.Find(accountId);
            var command = commandParser.Parse(commandText);

            return command?.Execute(account)
                   ?? $"I do not know what [{commandText}] means.";
        }

        //

        private readonly AccountRepository accountRepository;
        private readonly CommandParser commandParser;
    }
}